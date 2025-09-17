# ��ʽϵͳʹ��ָ�� 2025.9.18

## ����
����Ŀ�Ѿ�������ͳһ����ʽϵͳ��������ɫ���塢��ť��ʽ�ʹ�����ʽ��ȷ������Ӧ�õ��Ӿ�һ���ԡ�

## �ļ��ṹ
```
ImageToMidi-v2/
������ Styles/
��   ������ Colors.axaml          # ��ɫ��Դ����
��   ������ ButtonStyles.axaml    # ��ť��ʽ
��   ������ WindowStyles.axaml    # ���ںͲ�����ʽ
������ App.axaml                 # ȫ����ʽ����
������ Views/
    ������ Windows/
        ������ MainWindow.axaml  # ʹ����ʽ��ʾ��
```

## ��ɫ��Դ (Colors.axaml)

### ��Ҫ��ɫ
- `PrimaryBackgroundColor` (#263238) - ������ɫ
- `SecondaryBackgroundColor` (#37474F) - �μ�����ɫ
- `PrimaryButtonColor` (#3F51B5) - ����ť��ɫ
- `PrimaryButtonHoverColor` (#717EC8) - ��ť��ͣ��ɫ
- `PrimaryTextColor` (#FFFFFF) - ��������ɫ

### ʹ�÷�ʽ
```xml
<!-- ʹ����ɫ��Դ -->
<Button Background="{StaticResource PrimaryButtonBrush}" 
        Foreground="{StaticResource PrimaryTextBrush}"/>
```

## ��ť��ʽ (ButtonStyles.axaml)

### ��Ҫ��ť��ʽ
ʹ�� `primary-button` ������
```xml
<Button Content="��ť����" 
        Classes="primary-button" 
        Command="{Binding YourCommand}"/>
```

### ���ڿ��ư�ť��ʽ
ʹ�� `window-control-button` ������
```xml
<Button Classes="window-control-button"
        Background="{StaticResource MinimizeButtonBrush}"
        Click="MinimizeButton_Click"/>
```

## ������ʽ (WindowStyles.axaml)

### ��������ʽ
```xml
<Window Classes="main-window" ...>
```

### ��������ʽ
```xml
<Border Classes="title-bar" ...>
    <TextBlock Classes="title-text" Text="���ڱ���"/>
    <Image Classes="app-icon" Source="/Assets/icon.png"/>
</Border>
```

### ����������ʽ
```xml
<Grid Classes="main-content">
    <TextBlock Classes="content-placeholder" Text="ռλ������"/>
</Grid>
```

## ���´�����ʹ����ʽ

### 1. �����´���ʱ
```xml
<Window xmlns="https://github.com/avaloniaui"
        Classes="main-window"
        Title="�´���">
    
    <!-- ������ -->
    <Border Classes="title-bar">
        <!-- ���������� -->
    </Border>
    
    <!-- ������ -->
    <Grid Classes="main-content">
        <!-- ʹ����Ҫ��ť -->
        <Button Content="������ť" Classes="primary-button"/>
    </Grid>
</Window>
```

### 2. ���û��ؼ���ʹ��
```xml
<UserControl xmlns="https://github.com/avaloniaui">
    <StackPanel>
        <!-- ʹ��ͳһ�İ�ť��ʽ -->
        <Button Content="����" Classes="primary-button"/>
        <Button Content="ȡ��" Classes="primary-button"/>
        
        <!-- ʹ��ͳһ��������ʽ -->
        <TextBlock Classes="content-placeholder" Text="��ʾ��Ϣ"/>
    </StackPanel>
</UserControl>
```

## ��չ��ʽϵͳ

### �������ɫ
�� `Colors.axaml` ����ӣ�
```xml
<Color x:Key="NewColor">#RRGGBB</Color>
<SolidColorBrush x:Key="NewBrush" Color="{StaticResource NewColor}"/>
```

### ����°�ť��ʽ
�� `ButtonStyles.axaml` ����ӣ�
```xml
<Style Selector="Button.new-button-style">
    <Setter Property="Background" Value="{StaticResource NewBrush}"/>
    <!-- ������������ -->
</Style>
```

### ����´�����ʽ
�� `WindowStyles.axaml` ����ӣ�
```xml
<Style Selector="Window.new-window-style">
    <Setter Property="Background" Value="{StaticResource NewBrush}"/>
    <!-- ������������ -->
</Style>
```

## ����

1. **һ����** - ����Ӧ��ʹ��ͳһ���Ӿ����
2. **ά����** - ֻ���޸���ʽ�ļ����ɸ�������Ӧ�õ����
3. **������** - ��ʽ�����ڶ�����ںͿؼ����ظ�ʹ��
4. **����չ��** - ������������µ���ʽ����Ӱ�����д���

## ע������

1. �޸���ɫ��Դʱ����Ӱ������Ӧ�õ����
2. ������ʽ��ǵ�����Ҫ�ĵط������Ӧ�� `Classes` ����
3. ��ʽ�ļ��Ѿ��Զ���������Ŀ�����У������ֶ��������
4. ��ʽ�����ȼ���������ʽ > ȫ����ʽ > Ĭ����ʽ