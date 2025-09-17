# 样式系统使用指南 2025.9.18

## 概述
本项目已经建立了统一的样式系统，包含颜色定义、按钮样式和窗口样式，确保整个应用的视觉一致性。

## 文件结构
```
ImageToMidi-v2/
├── Styles/
│   ├── Colors.axaml          # 颜色资源定义
│   ├── ButtonStyles.axaml    # 按钮样式
│   └── WindowStyles.axaml    # 窗口和布局样式
├── App.axaml                 # 全局样式引用
└── Views/
    └── Windows/
        └── MainWindow.axaml  # 使用样式的示例
```

## 颜色资源 (Colors.axaml)

### 主要颜色
- `PrimaryBackgroundColor` (#263238) - 主背景色
- `SecondaryBackgroundColor` (#37474F) - 次级背景色
- `PrimaryButtonColor` (#3F51B5) - 主按钮颜色
- `PrimaryButtonHoverColor` (#717EC8) - 按钮悬停颜色
- `PrimaryTextColor` (#FFFFFF) - 主文字颜色

### 使用方式
```xml
<!-- 使用颜色资源 -->
<Button Background="{StaticResource PrimaryButtonBrush}" 
        Foreground="{StaticResource PrimaryTextBrush}"/>
```

## 按钮样式 (ButtonStyles.axaml)

### 主要按钮样式
使用 `primary-button` 类名：
```xml
<Button Content="按钮文字" 
        Classes="primary-button" 
        Command="{Binding YourCommand}"/>
```

### 窗口控制按钮样式
使用 `window-control-button` 类名：
```xml
<Button Classes="window-control-button"
        Background="{StaticResource MinimizeButtonBrush}"
        Click="MinimizeButton_Click"/>
```

## 窗口样式 (WindowStyles.axaml)

### 主窗口样式
```xml
<Window Classes="main-window" ...>
```

### 标题栏样式
```xml
<Border Classes="title-bar" ...>
    <TextBlock Classes="title-text" Text="窗口标题"/>
    <Image Classes="app-icon" Source="/Assets/icon.png"/>
</Border>
```

### 内容区域样式
```xml
<Grid Classes="main-content">
    <TextBlock Classes="content-placeholder" Text="占位符文字"/>
</Grid>
```

## 在新窗口中使用样式

### 1. 创建新窗口时
```xml
<Window xmlns="https://github.com/avaloniaui"
        Classes="main-window"
        Title="新窗口">
    
    <!-- 标题栏 -->
    <Border Classes="title-bar">
        <!-- 标题栏内容 -->
    </Border>
    
    <!-- 主内容 -->
    <Grid Classes="main-content">
        <!-- 使用主要按钮 -->
        <Button Content="操作按钮" Classes="primary-button"/>
    </Grid>
</Window>
```

### 2. 在用户控件中使用
```xml
<UserControl xmlns="https://github.com/avaloniaui">
    <StackPanel>
        <!-- 使用统一的按钮样式 -->
        <Button Content="保存" Classes="primary-button"/>
        <Button Content="取消" Classes="primary-button"/>
        
        <!-- 使用统一的文字样式 -->
        <TextBlock Classes="content-placeholder" Text="提示信息"/>
    </StackPanel>
</UserControl>
```

## 扩展样式系统

### 添加新颜色
在 `Colors.axaml` 中添加：
```xml
<Color x:Key="NewColor">#RRGGBB</Color>
<SolidColorBrush x:Key="NewBrush" Color="{StaticResource NewColor}"/>
```

### 添加新按钮样式
在 `ButtonStyles.axaml` 中添加：
```xml
<Style Selector="Button.new-button-style">
    <Setter Property="Background" Value="{StaticResource NewBrush}"/>
    <!-- 其他属性设置 -->
</Style>
```

### 添加新窗口样式
在 `WindowStyles.axaml` 中添加：
```xml
<Style Selector="Window.new-window-style">
    <Setter Property="Background" Value="{StaticResource NewBrush}"/>
    <!-- 其他属性设置 -->
</Style>
```

## 优势

1. **一致性** - 整个应用使用统一的视觉风格
2. **维护性** - 只需修改样式文件即可更新整个应用的外观
3. **复用性** - 样式可以在多个窗口和控件中重复使用
4. **可扩展性** - 可以轻松添加新的样式而不影响现有代码

## 注意事项

1. 修改颜色资源时，会影响整个应用的外观
2. 新增样式后记得在需要的地方添加相应的 `Classes` 属性
3. 样式文件已经自动包含在项目构建中，无需手动添加引用
4. 样式的优先级：本地样式 > 全局样式 > 默认样式