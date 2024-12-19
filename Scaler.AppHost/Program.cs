var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.Scaler_Product>("scaler-product");
builder.AddProject<Projects.Scaler_Notification>("scaler-notification");
builder.AddProject<Projects.scaler_client>("scaler-client");
builder.AddProject<Projects.Scaler_Server>("scaler-server");


builder.Build().Run();
