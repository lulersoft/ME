# ME

<b>ME_Unity3D-NLua是一个基于unity+nlua技术的全LUA免费开源（带有资源下载功能）游戏框架</b><br>

基于https://github.com/Mervill/Unity3D-NLua<br>

整理：小陆(QQ2604904)<br>

// ----------------框架目标-----------<br>

<b>使用lua 编写2D,3D全平台运行游戏，一次编写，到处发布</b><br>

//-----------------安装说明------------ Assets/Engine<br> 为本框所有架核心代码，其它请和https://github.com/Mervill/Unity3D-NLua保持一致 Assets/data/lua为 lua脚本代码<br>

wwwroot目录为web服务器目录，请上传到您的web服务器 请修改version内json参数,status： 0 无更新，1有更新包;force 1强制更新,0不强制更新,downrul 更新包路径<br>

u3d工程目录内:<br>
Assets/data为资源文件，所有打包的文件存放这里(含lua)。 <br>
Assets/StreamingAssets 存放的是生成好的更新包(data.zip),内容为data资源文件夹。<br>
生成后，请把一份上传到wwwroot服务器目录内，供客户端下载更新。<br>

测试场景 Engine/Scene/Demo.unity<br>

首次安装会从 StreamingAssets 中复制资源文件data.zip并解压到可读目录，然后加载主入口文件main.lua执行 所有逻辑尽可能在lua端执行 如：在lua中进行版本检测，下载zip，解压覆盖以后每次运行，会检测远程服务器版本，如果有更新，则下载，并自动解压覆盖资源目录。<br>


特性：<br>
全lua代码编写游戏。<br>
unity3d editor编辑器负责可视化资源的创建和打包。<br>
使用内置ugui来做UI甚至游戏。<br>
热更新更彻底：可通过热更新来更新成不同类型的游戏。<br>
通过纯lua“打地鼠”小游戏实战来提高框架的实用性.<br>

更新历史:<br>

//-------------2015-2-1--------------- 版本 0.0.2<br>

(1)添加Lua事件中心 AddListener|RemoveListener|Broadcast 可传table参数,彻底解耦 <br>
(2)修改为全局共用一个lua state，据说能提高效率和节省不必要的开销。。。 <br>
(3)添加Lua 异步下载 封装 <br>
(4)添加Lua 异步HTTP请求 封装<br>
(5)添加Lua 定时器<br>
(6)添加Lua 协程<br>
(7)添加“打地鼠”游戏demo<br>
(8)添加资源总管（哈希表管理资源)<br>
(9)移除ngui,改用ugui<br>

//-------------2015-1-18--------------- 版本 0.0.1<br>
带zip资源生成和下载发布<br>
