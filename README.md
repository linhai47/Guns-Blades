# 

# \# ⚔️ Guns\&Blades

# 

# \### 基于 Kerberos 认证协议的分布式联机游戏原型

# 

# \## 📖 项目简介

# 

# 本项目是一个集成了\*\*网络安全认证\*\*与\*\*实时同步\*\*的分布式系统原型。通过手搓 Socket 通信，实现了一个包含身份认证（AS）、票据授权（TGS）以及多节点游戏服务端（V）的完整 C/S 架构联机游戏。

# 

# \## 🏗 系统架构

# 

# 项目采用逻辑分布式的 \*\*C/S (Client-Server)\*\* 架构，各节点通过 TCP Socket 进行通信，数据交换格式统一为 \*\*JSON\*\*。

# 

# \### 节点说明：

# 

# 1\.  \*\*Client (Unity Game):\*\* 玩家客户端，负责状态机控制、预测逻辑及 UI 展示。

# 2\.  \*\*AS (Authentication Server):\*\* 身份认证服务器，验证用户凭证并分发 TGT（票据授予票据）。

# 3\.  \*\*TGS (Ticket Granting Server):\*\* 票据授予服务器，验证 TGT 并分发针对特定游戏服务器的 Service Ticket。

# 4\.  \*\*V / Game Server (Server):\*\* 游戏逻辑服务器，验证票据合法性后，负责广播玩家位置、动作等同步数据。

# 

# \## 🔒 安全协议流程 (Kerberos)

# 

# 1\.  \*\*认证阶段:\*\* Client 向 AS 发送加密的用户请求，AS 验证后返回由 Client 密钥加密的 TGT。

# 2\.  \*\*授权阶段:\*\* Client 带着 TGT 向 TGS 请求游戏服务，TGS 返回针对特定 Game Server 的 Service Ticket。

# 3\.  \*\*接入阶段:\*\* Client 带着 Service Ticket 连接 Game Server，Server 验证成功后允许玩家进入游戏世界。

# 

# \## 📡 通信协议规范

# 

# \### 数据封装 (LTV 模式)

# 

# 为了解决 TCP 粘包/半包问题，所有 JSON 数据包均采用长度首部封装：

# `\[ 4 字节长度头 | JSON 负载数据 ]`

# 

# \### 核心数据结构 (示例)

# 

# \*\*位置同步消息:\*\*

# 

# ```json

# {

# &#x20; "type": "SYNC\_POSITION",

# &#x20; "player\_id": "knight\_01",

# &#x20; "pos\_x": 12.5,

# &#x20; "pos\_y": -3.2,

# &#x20; "state": "RUN"

# }

# ```

# 

# \## 🛠️ 技术栈

# 

# &#x20; \* \*\*前端:\*\* Unity 6000.1 (URP), New Input System, FSM (有限状态机)

# &#x20; \* \*\*后端:\*\* C\\# .NET 8 / Python 3.10 (Socket 编程)

# &#x20; \* \*\*通信:\*\* TCP 协议 + JSON 序列化

# &#x20; \* \*\*加密:\*\* AES (对称加密), SHA-256 (数据完整性)

# 

# \## 📂 项目目录结构

# 

# ```text

# .

# ├── Client-Unity/          # Unity 客户端工程

# │   ├── Assets/Scripts/    # 核心逻辑（状态机、网络处理器）

# │   └── ...

# ├── Server-AS/             # 认证服务器代码

# ├── Server-TGS/            # 票据服务器代码

# ├── Server-Game/           # 游戏逻辑同步服务器

# └── Shared/                # 协议定义与 JSON 模板

# ```

# 

# \-----

# 

# AI给的建议

# 1\.  \*\*关于 IP 配置：\*\* 既然你打算用 3 台机器跑 5 个节点，建议你在文档里加一个 `Config` 表，明确写出 AS 占哪个 IP 哪个端口，TGS 占哪个 IP 哪个端口。这样你队友写代码时才不会乱。

# 2\.  \*\*关于 JSON 库：\*\*

# &#x20;     \* 如果服务端用 \*\*C\\#\*\*，推荐使用 `Newtonsoft.Json` (Json.NET)，这是行业标准。

# &#x20;     \* 如果服务端用 \*\*Python\*\*，直接用内置的 `import json` 即可。

# 

# 

