# HttPlaceholder.Client.Api.StubApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**StubAdd**](StubApi.md#stubadd) | **POST** /ph-api/stubs | Adds a new stub.
[**StubDelete**](StubApi.md#stubdelete) | **DELETE** /ph-api/stubs/{StubId} | Delete a specific stub by stub identifier.
[**StubDeleteAll**](StubApi.md#stubdeleteall) | **DELETE** /ph-api/stubs | Delete ALL stubs. Be careful.
[**StubGet**](StubApi.md#stubget) | **GET** /ph-api/stubs/{StubId} | Get a specific stub by stub identifier.
[**StubGetAll**](StubApi.md#stubgetall) | **GET** /ph-api/stubs | Get all stubs.
[**StubGetOverview**](StubApi.md#stubgetoverview) | **GET** /ph-api/stubs/overview | Get stub overview.
[**StubGetRequestsByStubId**](StubApi.md#stubgetrequestsbystubid) | **GET** /ph-api/stubs/{StubId}/requests | Get requests for the given stub ID.
[**StubUpdate**](StubApi.md#stubupdate) | **PUT** /ph-api/stubs/{StubId} | Updates a given stub.


<a name="stubadd"></a>
# **StubAdd**
> FullStubDto StubAdd (StubDto stubDto)

Adds a new stub.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class StubAddExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new StubApi(config);
            var stubDto = new StubDto(); // StubDto | 

            try
            {
                // Adds a new stub.
                FullStubDto result = apiInstance.StubAdd(stubDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StubApi.StubAdd: " + e.Message );
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
 **stubDto** | [**StubDto**](StubDto.md)|  | 

### Return type

[**FullStubDto**](FullStubDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | OK, with the created stub |  -  |
| **409** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="stubdelete"></a>
# **StubDelete**
> void StubDelete (string stubId)

Delete a specific stub by stub identifier.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class StubDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new StubApi(config);
            var stubId = stubId_example;  // string | 

            try
            {
                // Delete a specific stub by stub identifier.
                apiInstance.StubDelete(stubId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StubApi.StubDelete: " + e.Message );
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
 **stubId** | **string**|  | 

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
| **204** | OK, but not content |  -  |
| **404** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="stubdeleteall"></a>
# **StubDeleteAll**
> void StubDeleteAll ()

Delete ALL stubs. Be careful.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class StubDeleteAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new StubApi(config);

            try
            {
                // Delete ALL stubs. Be careful.
                apiInstance.StubDeleteAll();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StubApi.StubDeleteAll: " + e.Message );
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
| **204** | OK, but not content |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="stubget"></a>
# **StubGet**
> FullStubDto StubGet (string stubId)

Get a specific stub by stub identifier.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class StubGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new StubApi(config);
            var stubId = stubId_example;  // string | 

            try
            {
                // Get a specific stub by stub identifier.
                FullStubDto result = apiInstance.StubGet(stubId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StubApi.StubGet: " + e.Message );
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
 **stubId** | **string**|  | 

### Return type

[**FullStubDto**](FullStubDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | The stub. |  -  |
| **404** |  |  -  |
| **0** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="stubgetall"></a>
# **StubGetAll**
> List&lt;FullStubDto&gt; StubGetAll ()

Get all stubs.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class StubGetAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new StubApi(config);

            try
            {
                // Get all stubs.
                List<FullStubDto> result = apiInstance.StubGetAll();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StubApi.StubGetAll: " + e.Message );
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
| **200** | All stubs. |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="stubgetoverview"></a>
# **StubGetOverview**
> List&lt;FullStubOverviewDto&gt; StubGetOverview ()

Get stub overview.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class StubGetOverviewExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new StubApi(config);

            try
            {
                // Get stub overview.
                List<FullStubOverviewDto> result = apiInstance.StubGetOverview();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StubApi.StubGetOverview: " + e.Message );
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

[**List&lt;FullStubOverviewDto&gt;**](FullStubOverviewDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | All stubs. |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="stubgetrequestsbystubid"></a>
# **StubGetRequestsByStubId**
> List&lt;RequestResultDto&gt; StubGetRequestsByStubId (string stubId)

Get requests for the given stub ID.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class StubGetRequestsByStubIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new StubApi(config);
            var stubId = stubId_example;  // string | 

            try
            {
                // Get requests for the given stub ID.
                List<RequestResultDto> result = apiInstance.StubGetRequestsByStubId(stubId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StubApi.StubGetRequestsByStubId: " + e.Message );
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
 **stubId** | **string**|  | 

### Return type

[**List&lt;RequestResultDto&gt;**](RequestResultDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | request results for the given stubId |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="stubupdate"></a>
# **StubUpdate**
> void StubUpdate (string stubId, StubDto stubDto)

Updates a given stub.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class StubUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new StubApi(config);
            var stubId = stubId_example;  // string | The stub ID.
            var stubDto = new StubDto(); // StubDto | The posted stub.

            try
            {
                // Updates a given stub.
                apiInstance.StubUpdate(stubId, stubDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StubApi.StubUpdate: " + e.Message );
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
 **stubId** | **string**| The stub ID. | 
 **stubDto** | [**StubDto**](StubDto.md)| The posted stub. | 

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
| **204** | OK, but no content returned |  -  |
| **409** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

