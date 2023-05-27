# DynamicWallpaper
使用ChatGPT生成的自动更换壁纸的小程序

[![.NET Core Desktop](https://github.com/gmij/DynamicWallpaper/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/gmij/DynamicWallpaper/actions/workflows/dotnet-desktop.yml)


## 目前的功能
1. 支持多显示器随机设置壁纸
2. 支持从Bing以及Pixabay获取新壁纸
3. 提供了网络壁纸接口，用于后续扩展
4. 使用开盲盒的方式，来获取新的壁纸，增加趣味性。
5. 每次开盲盒时，开出来的数量加入随机因子。
6. 本地壁纸数量限制为100张，减少内存和磁盘的占用。自动清除最先下载的图片（后续版本中，考虑加入锁定功能，锁定的图片不能清除）
7. 

## 接下来考虑新增的功能
1. 提供更多的壁纸提供商
2. 加入许愿的功能，可以自定义喜欢的主题（风景，明星等）


## GPT使用情况
1. 让GPT提供具有API接口的高清壁纸网站
2. 根据1的接口，创建数据模型
3. winform控件在操作时闪烁，开启双缓冲（默认提供的方法，不太可行）
4. 让GPT提供免费的ICON图标网站
5. 获取当前计算机的显示器设备信息的实现方式
6. 开源方案中，可以切换windows壁纸的方案及调用方式（原来想用windows原生的能力，最后还是使用了Com+的能力。仅支持win7+）
7. winform窗口编程中，出现的多线程调度故障的解决方案

## GPT使用过程中的不足
1. 无法对整体代码进行业务设计
2. 对程序的抽象不足，更偏向功能袜，业务抽象部分还是需要依赖人工完成。

## 点赞的工具
1. Cursor，确实好用，给单文件代码做了不少建议性方案，但有时也很呆。
2. XX-Net，懂的都懂哈。
3. Copilot Chat，目前对中文支持，不是很友好，经常提问后，就串到英文回复。不过好在的一点是和VS深度集成，在调试时，能对异常给出比较好的修复建议。
