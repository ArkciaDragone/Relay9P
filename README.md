# Relay9P

**九张羊皮纸**网络中继工具

真的好用！

---

A *Nine Parchments* network relay tool

Simply works!

# 太长不看 TL;DR

在 Releases 页面下载 Relay9P.zip，解压后文件夹放到任意位置，点击 Relay9P.exe 运行即可。

---

Download Relay9P.zip from the Releases page and run Relay9P.exe.

# 故事 Story

Frozenbyte 家游戏的画面有多美，联机性能就有多烂，这已经不是一天两天了。Steam 上中文的差评基本全都是联机断线。

然而，令人沮丧的断线背后，却是非常尴尬的网络逻辑：玩家要给服务器发心跳包，收不到就算断线，即使玩家之间互相的通讯和远在（目前是）荷兰的服务器没有关系。

但是，总有这样可歌可泣的英雄，会去抓包写程序：[BSoD123456 的联机脚本](https://gist.github.com/BSoD123456/4161c185e4f9d0bfa6a4b5ea41cab21f)。考虑到它是一个 python 2 的实现，不适用于非程序员玩家自己运行。因此就写了一个 C# 版本给 windows 玩家使用，一并附上了改 hosts 的功能。

---

Frozenbyte has produced so many games with exquisite graphics and terrible online connections, as least for Chinese players far-away from their main server (currently) located in the Netherlands.

It is such a shame that they would ruin the gaming experience by an awkwardly counterintuitive connection system design: All players must report to the main server at regular intervals, otherwise they will be considered disconnected, even if the connection between players is still intact and the game is going on without any problems.

If you have connections issues with the main server as well, it should help to try this tool.

This tool is adapted from [this python 2 script by BSoD123456](https://gist.github.com/BSoD123456/4161c185e4f9d0bfa6a4b5ea41cab21f).

# 原理 Principle

中继所有客户端给主服务器的数据，但是立刻回复客户端发送的心跳包；忽略接下来主服务器可能返回也可能不返回的心跳包。

为了做到这一点，需要让游戏向此程序发送数据，通过修改 hosts 文件重定向发给主服务器域名的 UDP 数据包到本机。

为了修改 hosts 文件，需要管理员权限；为了进行网络中转，需要通过防火墙。

当然，如果想跳过赋予管理员权限这一步，也可以手动修改 hosts 文件，加入下面一行即可：

```
127.0.0.1 master.frozenbyte-online.com
```

---

Relay all data from clients to the main server, but immediately reply to heartbeat packets sent by clients; Ignore heartbeat packets that may or may not be returned by the main server.

To achieve this, it is necessary for the game to send data to this program and redirect UDP packets sent to the main server domain name to the local machine by modifying the hosts file.

In order to modify the hosts file, this application requires administrator privileges (first run only); In order to facilitate network transfer, this application needs to pass through the firewall.

If you prefer not to grant administrator privileges, you can manually modify the hosts file by adding the following line:
```
127.0.0.1 master.frozenbyte-online.com
```

# 下载 Downloads

Github Releases 中提供了已编译的两个版本：
- **Relay9P.zip**：如果你不知道该用哪个，这个就可以
- Relay9P.dotnet.7.zip：文件体积很小，但是需要.Net 7运行时

可以[从微软获取 .Net 7 运行时](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)。

---

Github Releases provides two compiled versions:
- **Relay9P.zip**：Download this if you are not sure which one to use
- Relay9P.dotnet.7.zip：Smaller size but requires .Net 7 runtime

The .Net 7 runtime can be downloaded from [here](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)。

# 注意 Note

这个工具只能解决由于与主服务器连接不良导致的断线。这样的断线往往表现为“与主机的连接断开”（或者类似的字样，记不得了）。

如果你和你的朋友之间的网络不畅，不能由这个工具解决。使用此工具后，没有理由使用加速器，除非加速器能帮助你和你朋友之间的互联。

尽管自从用了这个工具我就再没有掉线过，它也不能解决所有的问题，比如一发被队友的大火球炸死（？）。

This tool can only prevent disconnection caused by poor connection to the main server.

If the network between you and your friends is not smooth, this tool can do nothing about it. Consider changing your network.

Although I have never ever lost connection again since I deployed this tool, it can't solve all problems, such as being killed by your friends' single fireball.