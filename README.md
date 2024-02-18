# README #
AccountsSyncData is used to sync data from Domain Events to ShopGo Database.

### Environment Variables ###
memoryLimitMiB            (Memory for the lambda function)
timeOut                   (Lambda execution time out setting)
ConcurrentExecutions      (Concurrent instances of Lambada)
ScalingMinCapacity          
ScalingMaxCapacity          
ScalingUtilizationTarget    
MaximumMessageCount       (maximum messages of one batch Lambda can handle)

### Build and Test ###
cd .\src\AccountsSyncData\
dotnet build

cd .\test\AccountsSyncData.Tests\
dotnet test