# Ave.Extensions.Console.StateManagement

[![Build Status](https://versteeg-its.visualstudio.com/Ave/_apis/build/status/CI%20-%20Ave.Extensions.Console.CommandLine?branchName=master)](https://versteeg-its.visualstudio.com/Ave/_build/latest?definitionId=130&branchName=master)
[![Nuget downloads](https://img.shields.io/nuget/v/Ave.Extensions.Console.CommandLine)](https://www.nuget.org/packages/Ave.Extensions.Console.CommandLine/)
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/aadversteeg/ave.extensions.console.commandline/blob/master/LICENSE)

Nuget package for managing state of console application.

Console applications lose state after terminating. This package enables to preserve state between multiple invocations of a console application in the same console window.

The process id of the parent, ussually the console window, is used as a key to store the state. The state is stored as a binary file. The contents of this file can be protected using the Data Protection API (DPAPI).
