# HttPlaceholder.Client.Api.TenantApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**TenantDeleteAll**](TenantApi.md#tenantdeleteall) | **DELETE** /ph-api/tenants/{Tenant}/stubs | Deletes all stubs in a specific tenant.
[**TenantGetAll**](TenantApi.md#tenantgetall) | **GET** /ph-api/tenants/{Tenant}/stubs | Gets all stubs in a specific tenant.
[**TenantGetTenantNames**](TenantApi.md#tenantgettenantnames) | **GET** /ph-api/tenants | Gets all available tenant names.
[**TenantUpdateAll**](TenantApi.md#tenantupdateall) | **PUT** /ph-api/tenants/{Tenant}/stubs | Updates the stubs in a specific tenant with the posted values. If a stub that is currently available in a tenant isn&#39;t sent in the request, it will be deleted.


<a name="tenantdeleteall"></a>
# **TenantDeleteAll**
> void TenantDeleteAll (string tenant)

Deletes all stubs in a specific tenant.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class TenantDeleteAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new TenantApi(config);
            var tenant = tenant_example;  // string | 

            try
            {
                // Deletes all stubs in a specific tenant.
                apiInstance.TenantDeleteAll(tenant);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantApi.TenantDeleteAll: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **tenant** | **string**|  | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **204** | OK, but no content |  -  |
| **0** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="tenantgetall"></a>
# **TenantGetAll**
> List&lt;FullStubDto&gt; TenantGetAll (string tenant)

Gets all stubs in a specific tenant.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class TenantGetAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new TenantApi(config);
            var tenant = tenant_example;  // string | 

            try
            {
                // Gets all stubs in a specific tenant.
                List<FullStubDto> result = apiInstance.TenantGetAll(tenant);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantApi.TenantGetAll: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **tenant** | **string**|  | 

### Return type

[**List&lt;FullStubDto&gt;**](FullStubDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | All stubs in the tenant. |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="tenantgettenantnames"></a>
# **TenantGetTenantNames**
> List&lt;string&gt; TenantGetTenantNames ()

Gets all available tenant names.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class TenantGetTenantNamesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new TenantApi(config);

            try
            {
                // Gets all available tenant names.
                List<string> result = apiInstance.TenantGetTenantNames();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantApi.TenantGetTenantNames: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters
This endpoint does not need any parameter.

### Return type

**List<string>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | All available tenant names. |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="tenantupdateall"></a>
# **TenantUpdateAll**
> void TenantUpdateAll (string tenant, List<StubDto> stubDto)

Updates the stubs in a specific tenant with the posted values. If a stub that is currently available in a tenant isn't sent in the request, it will be deleted.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class TenantUpdateAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new TenantApi(config);
            var tenant = tenant_example;  // string | 
            var stubDto = new List<StubDto>(); // List<StubDto> | 

            try
            {
                // Updates the stubs in a specific tenant with the posted values. If a stub that is currently available in a tenant isn't sent in the request, it will be deleted.
                apiInstance.TenantUpdateAll(tenant, stubDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantApi.TenantUpdateAll: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **tenant** | **string**|  | 
 **stubDto** | [**List&lt;StubDto&gt;**](StubDto.md)|  | 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **204** | OK, but no content |  -  |
| **0** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

