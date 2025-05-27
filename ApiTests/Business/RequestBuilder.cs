namespace ApiTestingFramework.Business;

public RequestBuilder(string resource, Method method)
{
    _request = new RestRequest(resource, method);
}

    public RequestBuilder AddHeader(string name, string value)
    {
        _request.AddHeader(name, value);
        return this;
    }

    public RequestBuilder AddJsonBody(object body)
    {
        _request.AddJsonBody(body);
        return this;
    }

    public RestRequest Build()
    {
        return _request;
    }
}