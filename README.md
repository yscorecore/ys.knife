# YS.Knife

> `YS.Knife` 是基于.Net Core 依赖注入的一个拓展框架，能够使你基于注解的方式，更方便的使用依赖注入。

[![build](https://github.com/yscorecore/ys.knife/workflows/build/badge.svg)](https://github.com/yscorecore/ys.knife/actions?query=workflow%3Abuild) [![codecov](https://codecov.io/gh/yscorecore/ys.knife/branch/master/graph/badge.svg)](https://codecov.io/gh/yscorecore/ys.knife) [![Nuget](https://img.shields.io/nuget/v/YS.Knife.Core)](https://nuget.org/packages/ys.knife.Core/) [![GitHub](https://img.shields.io/github/license/yscorecore/ys.knife)](https://github.com/yscorecore/ys.knife/blob/master/LICENSE)

- 服务注入
    - 基本使用方法
    - 使用`IEnumerable<T>`类型的对象
    - 使用`IDictionary<string,T>`类型的对象
- 配置注入
    - 基本使用方法
    - 基于注解的配置项验证
- 自定义注入服务

    - 自定义特性
    - 使用`IServiceRegister`自定义注入服务