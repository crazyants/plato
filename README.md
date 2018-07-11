# Plato

## Introduction

Welcome to the official repository for Plato. Plato is a modern, multi-tenanted, modular .NET core framework being developed by InstantASP to support the next versions of both InstantForum & InstantKB. 

# Versions

Development Branch ( master )

# Getting Started

## Cloning the Plato Repository

To clone the Plato repository locally from a Git command line please use...

git clone --recursive https://github.com/InstantASP/plato.git

## Using visual Studio

Once you've cloned the Plato repository locally you can open the enclosed Plato.sln within Visual Studio 2017 or above to build & run the web application locally within IIS Express.

You can run the application from within Visual Studio and go through the set-up process. If you don't see the set-up page, please delete any folder within src/Plato/App_Data folder. Once the App_Data folder is empty no tenants will be registered so the initial set-up should be displayed.

During set-up you’ll need to provide a database connection string and a default administrator password. Please target SQL server for now. All tables and stored procedures will be created during the web based set-up process.

# License

&copy; All source code is copyright of InstantASP Limited.
