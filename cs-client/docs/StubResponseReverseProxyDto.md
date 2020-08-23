# HttPlaceholder.Client.Model.StubResponseReverseProxyDto
A model for storing reverse proxy settings.
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Url** | **string** | Gets or sets the URL where the request should be sent to. The request will be sent to exactly this URL. | [optional] 
**AppendQueryString** | **bool?** | Gets or sets whether the query string of the request to HttPlaceholder should be appended to the string that will be send to the proxy URL. | [optional] 
**AppendPath** | **bool?** | Gets or sets whether the path string of the request to HttPlaceholder should be appended to the string that will be send to the proxy URL. | [optional] 
**ReplaceRootUrl** | **bool?** | Gets or sets whether the root URL of the response of the target web service should be replaced with the root URL of HttPlaceholder. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

