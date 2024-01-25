## Create or migrate Database

```
cd MyHeart.Api
dotnet ef migrations add Name -p ../MyHeart.Data/ -c MyHeartContext
dotnet ef database update
```
