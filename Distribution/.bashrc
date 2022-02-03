if [[ ${START_X11} == 1 ]]; then
    unset START_X11
    exec startx
fi
