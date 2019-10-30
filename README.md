# Introduccion
Este repo contiene una libreria para el uso de cache en las apliaciones expone una interfaz ICacheStore con 3 proveedores RedisStore (redis), MemoryStore (memory), NullStore, este ultimo para poder deshabilitar la cache sin requerir cambios en codigo

# Uso
Para aplicaciones NetCore agregar el servicio AddCacheStores al IServiceCollection, requiere el objeto CacheStoreOptions que puede pasarse como IOptions<CacheStoreOptions> por ejemplo:

 ``` csharp
  public void ConfigureServices(IServiceCollection services)
  {
     ...
     var options = new CacheStoreOptions();

     _configuration.GetSection("CacheStoreOptions").Bind(options);

	 services.AddCacheStores(Microsoft.Extensions.Options.Options.Create(options)); //<-- Add this line
     ...
  }
  
```
Tambien es posible construir el objeto options como un Action<CacheStoreOptions>
 ``` csharp
  public void ConfigureServices(IServiceCollection services)
  {
     ...
	 services.AddCacheStores(opt =>
            {
                opt.CacheExpire = 30;
                opt.RedisHost = "localhost";
                opt.CacheStorage = "redis";
                ...
            });
     ...
  }

```
En el constructor de la clase que lo usara bastará con requerir la instancia ICacheStore
``` csharp
public class MyClass 
{
    private readonly ICacheStore cacheStore;
    private readonly IOptions<CacheStoreOptions> options;

    public MyClass(ICacheStore cacheStore, IOptions<CacheStoreOptions> options)
    {
        this.cacheStore = cacheStore;
        this.options = options;
    }
}

```
