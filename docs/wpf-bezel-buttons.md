# WPF bezel button colors

`BezelButton`, `BezelRepeatButton`, and `BezelToggleButton` use an opaque light face with dark text in the light theme, and an opaque dark face with light text in the dark theme. Their text backgrounds are transparent. The face, bevels, and state overlays update when the application theme changes.

The defaults use `ControlSolidFillColorDefaultBrush` for the face and `ButtonForeground` for text. `BezelButtonBorderBrush` and `BezelButtonInnerBorderBrush` define the outer and inner bevels; high-contrast themes use the system button text and face colors for these rims.

Set `Background` and `Foreground` to supply your own color pair. `BorderBrush`, `MinorBorderBrush1`, `PressedBrush`, and the toggle button's `MinorBackground1` also remain customizable. Explicit colors on a supplied `TextBlock` take precedence over the default text style.

```xml
<ui:BezelButton
    Content="Start"
    Background="DarkGreen"
    Foreground="White"
    Padding="12,6" />
```

When supplying a custom face color, choose a contrasting foreground color. `Padding` controls the space between the caption and the inner bevel for all three controls.
