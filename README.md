- 直接放入游戏文件夹下后打开 SSS_Prac_Launcher.exe
- 游戏版本需要为1.01版(https://www.bilibili.com/video/BV1seM4zWECM/)
- (理论上1.0.0可能也能用←Assembly Version改了 用不了 遗憾，，，)
- release文件中提供了一版

## 功能:
- 按 **backspace** 后, 注意这个游戏的backspace菜单是在右下角的
+ **F1** 可以开无敌
+ **F2** 后可禁用X键
+ **F3** 后切屏则不暂停
- 进入练习界面后可以选择从各种地方开始(甚至居然有ex，，，虽然道中只有前后半之分)
- 更多分辨率
+ 在**Setting.INI**文件中**Mode**下的**WindowSize**设置
+ WindowSize = 0: 640x480
+ WindowSize = 1: 800x600
+ WindowSize = 2: 1024x768
+ WindowSize = 3: 1280x960
+ WindowSize = 4: 1440x1080
+ WindowSize = 5: 1920x1440
+ WindowSize = 6: 2560x1920
+ WindowSize = 7: 2880x2160
+ 在游戏中使用 **F8** 调节(不过ui会错位, 所以还是用文件调节吧，，，)
- 剩下的还没做，，，

## 如何播放使用练习器的rep:
- 先在练习模式选择rep实际跳转到的章节并进入, 随后退出, 随后播放rep即可
- 当进入想要播放的章节后, 按backspace后应当看见第三行文字为 Prac=true
- 无需修改资源, 资源会自动继承

## 已知bug(特性(确信)):
- 为了保证某些东西, 这里强制使用DirectInput输入, 所以没法改键了

## 代码:
**https://github.com/RUEEE/ShiningShootingStar_Prac**


## 25.08.11
- (1.0.2.0)修复咕灵灵bug, 给终符加了阶段
- (1.0.2.0)修复backspace菜单在关闭的情况下也能响应热键
- (1.0.1.1) 加了更多的分辨率
- 修复了部分符卡时间错误
- 加入了禁用X键的功能
- 原来1.01版本还有个settings.ini文件，，，修复了没这个文件下无法启动的bug
- 修改了点配色之类的玩意

**蓝狗降临仪式**