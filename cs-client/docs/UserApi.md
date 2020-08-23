# HttPlaceholder.Client.Api.UserApi

All URIs are relative to *http://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**UserGet**](UserApi.md#userget) | **GET** /ph-api/users/{Username} | Get the user for the given username.


<a name="userget"></a>
# **UserGet**
> UserDto UserGet (string username)

Get the user for the given username.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using HttPlaceholder.Client.Api;
using HttPlaceholder.Client.Client;
using HttPlaceholder.Client.Model;

namespace Example
{
    public class UserGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new UserApi(config);
            var username = username_example;  // string | 

            try
            {
                // Get the user for the given username.
                UserDto result = apiInstance.UserGet(username);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserApi.UserGet: " + e.Message );
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
 **username** | **string**|  | 

### Return type

[**UserDto**](UserDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **401** |  |  -  |
| **200** | The User. |  -  |
| **403** |  |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

