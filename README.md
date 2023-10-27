<p align="center">
    <img src="Image/minori1.png" width="296" height="256" alt="minoribot">
</p>

<div align="center">

# MinoriBot

一个基于[go-cqhttp](https://github.com/Mrs4s/go-cqhttp)  的乱七八糟但是以Project Sekai游戏功能为主的QQ机器人

</div>

## 说明
本项目所用资源依赖于[sekai.best](https://sekai.best/)，数据库依赖于[SekaiViewer](https://github.com/Sekai-World/sekai-viewer)，部署时建议查看网络环境是否可以正常访问这些站点已避免Bot功能异常。

## 目标与进度

- [x] 查卡
- [x] 查活动
- [x] 查曲
- [ ] 查卡池
- [x] 查谱面  
- 由于不可抗力因素，正在适配Koishi框架

## 支持

### 接口

- [x] 正向WebSocket
- [x] 反向WebSocket

### cq事件

- [x] 群消息
- [ ] 私聊消息

## 相关指令
< >表示可多个关键词同时查找  
[ ]表示该关键词唯一  
sk查卡 <角色名> <团队名> <属性> <星级> [是否限定] [ID]  
sk查活动 <角色名> <团队名> <属性> [活动类型] [ID]  
sk查曲 <角色名> <团队名> [难度类型] [难度等级] [歌名部分内容] [ID]

## 部署方式

下载[go-cqhttp](https://github.com/Mrs4s/go-cqhttp)并登录QQ账号(因为tx原因目前登录需要一些技巧，暂时需要自行查找)  
更改Bot文件中的config.yaml来配置连接gocq的方式

### Windows

从Release中下载windows-x64版本，解压后双击exe即可

### Linux

安装.NET运行时(以Ubuntu为例，若已安装过运行时或SDK则无视这一步)
```
sudo apt-get install -y aspnetcore-runtime-6.0
```
从Release中下载linux-64版本，解压到任意位置，进入Bot根目录
```
cd /MinoriBot
```
为执行文件添加权限
```
chmod 755 MinoriBot
```
启动
```
./MinoriBot
```
后台运行：可以使用screen或者nohup保证Bot后台运行

### 其他说明

没有Release就是还没开发完  
第一次启动会比较慢因为要下载数据库文件

## 其他

- 摸鱼缓慢开发中。。。
- 文档等再完善一些功能再跟进
