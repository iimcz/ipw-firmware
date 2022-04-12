static fixed3 W = fixed3(0.2125, 0.7154, 0.0721);

float map(float val, float min1, float max1, float min2, float max2) {
  return min2 + (val - min1) * (max2 - min2) / (max1 - min1);
}

float brightnessCurve(fixed2 uv, float flipCurve) {
  if (flipCurve < 0.1) {
    if (uv.x > 0.5) {
      float progress = (uv.x - 0.5) * 2;
      return -lerp(0, 0.05, progress);
    }
  } else {
    if (uv.x < 0.5) {
      float progress = uv.x * 2;
      return -lerp(0.05, 0, progress);
    }
  }
  return 0.0;
}

// Source: http://paulbourke.net/miscellaneous/edgeblend/
float blendFunction(float x) {
  const float p = 2.0; // constant, but specified separately
  const float under_half = 0.5 * pow(2 * x, p);
  const float over_half = 1 - 0.5 * pow(2 * (1 - x), p);
  if (x < 0.5)
    return under_half;
  return over_half;
}

float gammaCorrection(float x, float gamma) {
  const float inv_gamma = 1.0 / gamma;
  return pow(x, inv_gamma);
}

float antiOverlap(fixed2 uv, float overlap, float flipCurve) {
  float x = clamp(uv.x, 0, 1);
  if (flipCurve < 0.5) {
    if (x > 1.0 - overlap) {
      float progress = map(x, 1.0 - overlap, 1.0, 1.0, 0.0);
      return blendFunction(clamp(progress, 0, 1));
    }
  } else {
    if (x < overlap) {
      float progress = map(x, 0.0, overlap, 0.0, 1.0);
      return blendFunction(clamp(progress, 0, 1));
    }
  }

  return 1.0;
}

fixed4 colorRamp(float val) {
  float lightness = (val % (1.0 / 4.0)) * 4.0;

  if (val < 0.25) {
    return fixed4(lightness, lightness, lightness, 1);
  } else if (val < 0.50) {
    return fixed4(1, lightness, lightness, 1);
  } else if (val < 0.75) {
    return fixed4(lightness, 1, lightness, 1);
  } else {
    return fixed4(lightness, lightness, 1, 1);
  }
}

fixed3 applyContrast(fixed3 input, float contrast) {
    return ((input - 0.5f) * max(contrast, 0)) + 0.5f;
}

fixed3 applySaturation(fixed3 input, float saturation) {
    float intComp = dot(input, W);
    fixed3 intensity = fixed3(intComp, intComp, intComp);
    return lerp(intensity, input, saturation);
}