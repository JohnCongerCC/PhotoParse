# Photo Parse
The purpose of this app parse all the photos on my drive and organize them

## To begin using this repo 
#### Follow these steps to set up this application
Note: This application requires dotnet core version 2.2
``` bash
  git clone https://github.com/JohnCongerCC/PhotoParse
  cd PhotoParse
  dotnet restore
```

## Build 
#### Follow these steps to build the app 
``` bash
  dotnet build
  dotnet ef migrations add Init (Not necessary this just creates the folder `Migrations`)
  dotnet ef database update
 ```

