# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="1.5.1"></a>
## [1.5.1](https://www.github.com/SaveApis/Streamer.Bot-Cloud/releases/tag/v1.5.1) (2025-09-23)

### 🐛 Bug Fixes

* Fix application sync ([#28](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/28)) ([ce7a230](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/ce7a230ccdb881fcd586672d28d374a476ce4c43))

<a name="1.5.0"></a>
## [1.5.0](https://www.github.com/SaveApis/Streamer.Bot-Cloud/releases/tag/v1.5.0) (2025-09-22)

### ✨ Features

* Application domain ([#27](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/27)) ([4ae6c72](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/4ae6c7251ad00c5bb698f9700edea6f8c4131b7b))

<a name="1.4.0"></a>
## [1.4.0](https://www.github.com/SaveApis/Streamer.Bot-Cloud/releases/tag/v1.4.0) (2025-09-18)

### ✨ Features

* Encryption module ([#26](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/26)) ([07d2175](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/07d21755dbf52527a02bb67ab70725a2ed417514))

### ♻️ Code Refactoring

* Only run MigrateDatabasesJob.cs if any dbcontext is available ([#25](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/25)) ([628231c](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/628231caf31daacd55391538004198a0d9c25275))

<a name="1.3.0"></a>
## [1.3.0](https://www.github.com/SaveApis/Streamer.Bot-Cloud/releases/tag/v1.3.0) (2025-09-17)

### ✨ Features

* EntityFrameworkCore Module ([#24](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/24)) ([fd2041a](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/fd2041aeebc4f427c2172727b9b34baa96a981cb))

<a name="1.2.0"></a>
## [1.2.0](https://www.github.com/SaveApis/Streamer.Bot-Cloud/releases/tag/v1.2.0) (2025-09-16)

### ✨ Features

* Hangfire Module ([#21](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/21)) ([60b9b3e](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/60b9b3e08814933db05b822d84ae753127fffbe6))
* Mediator Module ([#14](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/14)) ([4eb2107](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/4eb21075660046361b5185812985162538ce2642))
* Validation module ([#20](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/20)) ([1ca2b7b](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/1ca2b7b929e3a3c15774de853e0c3e5135622b99))

### 🐛 Bug Fixes

* Fix hangfire autofac dependency resolution exception ([#22](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/22)) ([146c413](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/146c413948b7df0ba125cbefa90af61cf7c26597))
* Fix hangfire autofac dependency resolution exception ([#23](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/23)) ([72f6985](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/72f698503b12dbfacadefd7939038a60333b0951))

<a name="1.1.0"></a>
## [1.1.0](https://www.github.com/SaveApis/Streamer.Bot-Cloud/releases/tag/v1.1.0) (2025-09-06)

### ✨ Features

* Correlation Module ([#10](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/10)) ([2d752b9](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/2d752b92b35a04910c93781cdc2eec0b35f9b516))
* Rest Module ([#9](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/9)) ([f4f0008](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/f4f00088a2e58c298538c73db198653f76f7f14e))
* Swagger module ([#11](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/11)) ([20cc1b7](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/20cc1b74e165c7f7cc55fc09b6efe4d920f297ff))

### ♻️ Code Refactoring

* Disable 'Try it out' button in swagger ([#13](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/13)) ([31d54df](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/31d54df849fd0904e1761ca345d715117b85f48c))

### 🐛 Bug Fixes

* Fix kubernetes ingress tls ([#12](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/12)) ([644e5a0](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/644e5a0dc8777b929d3dcbd38db29f182dde3bf6))

<a name="1.0.0"></a>
## [1.0.0](https://www.github.com/SaveApis/Streamer.Bot-Cloud/releases/tag/v1.0.0) (2025-09-06)

### ♻️ Code Refactoring

* Enable caching in dockerfile ([#5](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/5)) ([4f78840](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/4f7884057aab8d656de2854bda64f24cc3738701))

### 🐛 Bug Fixes

* Fix kubernetes deployment ([#7](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/7)) ([c453c9f](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/c453c9f8fdc8c6e3ede42d642d5112a6b87f7740))

### 👷 CI/CD

* CD Pipeline ([#8](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/8)) ([038cff3](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/038cff35d0493287482518c77074415c2516a5e4))
* CD Stage Pipeline ([#6](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/6)) ([22d801a](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/22d801ad6e1ad71afa2f3a90ce39e1702e59b5dd))
* CI pipelines ([#4](https://www.github.com/SaveApis/Streamer.Bot-Cloud/issues/4)) ([5ad18a9](https://www.github.com/SaveApis/Streamer.Bot-Cloud/commit/5ad18a9fe4f52a48d502d2b8f836fd957b71827e))

