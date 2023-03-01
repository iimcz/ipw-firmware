using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.ScriptsSdk.Extensions;
using emt_sdk.Packages;
using emt_sdk.Scene;
using emt_sdk.Settings;
using emt_sdk.Settings.IPW;
using Naki3D.Common.Protocol;
using UnityEngine;

public class ImageInfo
{
    public Vector2Int Position;
    public SpriteRenderer Renderer;
    public GameObject GameObject;
    public SlideComponent Slider;
    public BoxCollider2D Collider;
}

public class GalleryPoolComponent : MonoBehaviour
{
    public GameObject ImagePrefab;
    public CameraRigSpawnerComponent RigSpawner;

    public Gallery.GalleryLayoutEnum LayoutType;
    public List<ImageInfo> ImagePool; // No need to keep creating new images, keep a pool
    
    public GalleryLayout Layout { get; private set; }
    public List<Sprite> Sprites;

    public bool EnableInteraction = true;

    private IConfigurationProvider<PackageDescriptor> _packageProvider;

    IEnumerator Start()
    {
        _packageProvider = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<PackageDescriptor>>();
        
        // Wait two frames for the camera transformation to apply
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        ImagePool = new List<ImageInfo>();


        // Debug mode
        if (!_packageProvider.ConfigurationExists) SpawnDebugScene();
        else SpawnLoadedScene();
    }

    public void Apply(Gallery scene, string basePath)
    {
        if (ColorUtility.TryParseHtmlString(scene.BackgroundColor, out var backgroundColor) == false)
            throw new ArgumentException("Background color is not a valid HTML hex color string", nameof(scene.BackgroundColor));
        
        RigSpawner.CameraRig.SetBackgroundColor(backgroundColor);

        LayoutType = scene.LayoutType;
        CreateLayout();
        
        switch (scene.Layout)
        {
            case Gallery.ListLayout list:
                var listLayout = (GalleryListLayout) Layout;
                listLayout.Padding = scene.Padding.ToUnityVector();
                listLayout.Spacing = list.Spacing;
                listLayout.VisibleListLength = list.VisibleImages;
                listLayout.ScrollDelay = scene.ScrollDelay;
                
                Sprites = list.Images.Select(i =>
                {
                    var fileName = Path.Combine(basePath, i.FileName);
                    return GalleryLoader.LoadSprite(fileName);
                }).ToList();
                break;
            case Gallery.GridLayout grid:
                var gridLayout = (GalleryGridLayout)Layout;
                gridLayout.Padding = scene.Padding.ToUnityVector();
                gridLayout.Spacing = new UnityEngine.Vector2(grid.HorizontalSpacing, grid.VerticalSpacing);
                gridLayout.Columns = grid.Width;
                gridLayout.Rows = grid.Height;
                gridLayout.ScrollDelay = scene.ScrollDelay;

                Sprites = new List<Sprite>();
                foreach (var img in grid.Images)
                {
                    if (img == null) continue; // skip empty cells for non-fitting image counts
                    var fileName = Path.Combine(basePath, img.FileName);
                    Sprites.Add(GalleryLoader.LoadSprite(fileName));
                }
                break;
        }
        
        AllocatePool();
        Layout.Invalidate();
    }

    private void SpawnDebugScene()
    {
        Apply(new Gallery
        {
            BackgroundColor = "#000000",
            LayoutType = Gallery.GalleryLayoutEnum.List,
            Layout = new Gallery.ListLayout
            {
                Spacing = 0.1f,
                VisibleImages = 2,
                Images = new Gallery.GalleryImage[]
                {
                    new Gallery.GalleryImage
                    {
                        FileName = "test1.png"
                    },
                    new Gallery.GalleryImage
                    {
                        FileName = "test2.png"
                    },
                    new Gallery.GalleryImage
                    {
                        FileName = "test3.png"
                    }
                }
            },
            Padding = new Naki3D.Common.Protocol.Vector2Data
            {
                X = 0.1f,
                Y = 0.1f
            },
            ScrollDelay = 0f,
            SlideAnimationLength = 0.3f
        }, string.Empty);
    }

    private void SpawnLoadedScene()
    {
        var settings = _packageProvider.Configuration.Parameters.Settings;
        var layoutType = (Gallery.GalleryLayoutEnum)Enum.Parse(typeof(Gallery.GalleryLayoutEnum), settings.LayoutType.Value.ToString());

        Gallery.GalleryLayout MapLayoutType()
        {
            return layoutType switch
            {
                Gallery.GalleryLayoutEnum.List => new Gallery.ListLayout
                {
                    VisibleImages = (int)settings.Layout.VisibleImages,
                    Spacing = (float)settings.Layout.Spacing,
                    Images = settings.Layout.Images.Select(i => new Gallery.GalleryImage
                    {
                        ActivatedAction = i.ActivatedEvent,
                        SelectedAction = i.SelectedEvent,
                        FileName = i.FileName
                    }).ToArray()
                },
                Gallery.GalleryLayoutEnum.Grid => new Gallery.GridLayout
                {
                    Width = (int)settings.Layout.Width,
                    Height = (int)settings.Layout.Height,
                    HorizontalSpacing = (float)settings.Layout.HorizontalSpacing,
                    VerticalSpacing = (float)settings.Layout.VerticalSpacing,

                    // HACK: this is ugly, horrible and should be replaced
                    Images = new Func<Gallery.GalleryImage[,]>(() => {
                        var images = new Gallery.GalleryImage[(int)settings.Layout.Width, (int)settings.Layout.Height];
                        var intermediate = settings.Layout.Images.Select(i => new Gallery.GalleryImage
                        {
                            ActivatedAction = i.ActivatedEvent,
                            SelectedAction = i.SelectedEvent,
                            FileName = i.FileName
                        }).ToArray();

                        for (int i = 0; i < intermediate.Length; i++)
                        {
                            images[i % (int)settings.Layout.Width, i / (int)settings.Layout.Width] = intermediate[i];
                        }

                        return images;
                    })()
                },
                _ => throw new NotImplementedException(),
            };
        }

        Apply(new Gallery
        {
            BackgroundColor = settings.BackgroundColor,
            LayoutType = layoutType,
            Padding = new Naki3D.Common.Protocol.Vector2Data { X = (float)settings.Padding.X.Value, Y = (float)settings.Padding.Y.Value }, // TODO: I think we're generating the same type twice? Once from JSON schema, once from protobuf
            ScrollDelay = (float)settings.ScrollDelay.Value,
            SlideAnimationLength = (float)settings.SlideAnimationLength.Value,
            Layout = MapLayoutType()
        }, _packageProvider.Configuration.DataRoot);
    }

    void Update()
    {
        // Wait until layout is properly created
        if (Layout != null) Layout.Update();
    }

    private void AllocatePool()
    {
        foreach (var image in ImagePool) Destroy(image.GameObject);

        var poolSize = Layout.PoolSize;
        for (int i = 0; i < poolSize; i++)
        {
            var image = Instantiate(ImagePrefab, transform);
            var imageInfo = new ImageInfo
            {
                GameObject = image,
                Renderer = image.GetComponent<SpriteRenderer>(),
                Slider = image.GetComponent<SlideComponent>(),
                Collider = image.GetComponent<BoxCollider2D>(),
                Position = Vector2Int.zero
            };

            imageInfo.Renderer.sprite = Sprites[i % Sprites.Count];
            
            ImagePool.Add(imageInfo);
        }
    }

    public void CreateLayout()
    {
        Layout = LayoutType switch
        {
            Gallery.GalleryLayoutEnum.Grid => new GalleryGridLayout(this),
            Gallery.GalleryLayoutEnum.List => new GalleryListLayout(this),
            _ => throw new NotSupportedException(),
        };

        if (Layout is GalleryListLayout ll)
            ll.Orientation = 
                RigSpawner.CameraRig.Orientation == IPWSetting.IPWOrientation.Horizontal
                ? GalleryListLayout.GalleryListOrientation.Horizontal
                : GalleryListLayout.GalleryListOrientation.Vertical;
    }

    public void SingleLeft()
    {
        GestureEvent(HandGestureType.SwipeLeft);
    }

    public void SingleRight()
    {
        GestureEvent(HandGestureType.SwipeRight);
    }

    private void GestureEvent(HandGestureType gesture)
    {
        if (!EnableInteraction) return;
        Layout.Gesture(gesture);
    }

    public void KeyUpEvent(SensorDataMessage e)
    {
        if (!EnableInteraction) return;

        if (e.Integer == (int)KeyCode.LeftArrow) Layout.Previous();
        else if (e.Integer == (int)KeyCode.RightArrow) Layout.Next();
    }
}
