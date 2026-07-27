using System.ComponentModel;

namespace Egui.Containers;

/// <summary>
/// From <see cref="CollapsingState.ShowHeader"/>.
/// </summary>
public ref struct HeaderResponse
{
    public bool IsOpen => _state.IsOpen;

    private CollapsingState _state;
    private Ui _ui;
    private Response _toggleButtonResponse;
    private InnerResponse _response;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'HeaderResponse' does not contain a constructor that takes 0 arguments", error: true)]
    public HeaderResponse() { throw new InvalidOperationException(); }

    internal HeaderResponse(CollapsingState state, Ui ui, Response toggleButtonResponse, InnerResponse response)
    {
        _state = state;
        _ui = ui;
        _toggleButtonResponse = toggleButtonResponse;
        _response = response;
    }

    public void SetOpen(bool open)
    {
        _state.SetOpen(open);
    }

    public void Toggle()
    {
        _state.Toggle(_ui);
    }

    /// <summary>
    /// Returns the response of the collapsing button, the custom header, and the custom body.
    /// </summary>
    public (Response, InnerResponse, InnerResponse?) Body(Action<Ui> addBody)
    {
        var bodyResponse = _state.ShowBodyIndented(_response.Response, _ui, addBody);
        return (_toggleButtonResponse, _response, bodyResponse);
    }

    /// <inheritdoc cref="Body"/>
    public (Response, InnerResponse, InnerResponse<R>?) Body<R>(Func<Ui, R> addBody)
    {
        var bodyResponse = _state.ShowBodyIndented(_response.Response, _ui, addBody);
        return (_toggleButtonResponse, _response, bodyResponse);
    }

    /// <summary>
    /// Returns the response of the collapsing button, the custom header, and the custom body, without indentation.
    /// </summary>
    public (Response, InnerResponse, InnerResponse?) BodyUnindented(Action<Ui> addBody)
    {
        var bodyResponse = _state.ShowBodyUnindented(_ui, addBody);
        return (_toggleButtonResponse, _response, bodyResponse);
    }

    /// <inheritdoc cref="BodyUnindented"/>
    public (Response, InnerResponse, InnerResponse<R>?) BodyUnindented<R>(Func<Ui, R> addBody)
    {
        var bodyResponse = _state.ShowBodyUnindented(_ui, addBody);
        return (_toggleButtonResponse, _response, bodyResponse);
    }
}

/// <inheritdoc cref="HeaderResponse"/>
public ref struct HeaderResponse<H>
{
    public bool IsOpen => _state.IsOpen;

    private CollapsingState _state;
    private Ui _ui;
    private Response _toggleButtonResponse;
    private InnerResponse<H> _response;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("'HeaderResponse' does not contain a constructor that takes 0 arguments", error: true)]
    public HeaderResponse() { throw new InvalidOperationException(); }

    internal HeaderResponse(CollapsingState state, Ui ui, Response toggleButtonResponse, InnerResponse<H> response)
    {
        _state = state;
        _ui = ui;
        _toggleButtonResponse = toggleButtonResponse;
        _response = response;
    }

    internal HeaderResponse WithoutInner()
    {
        return new HeaderResponse(_state, _ui, _toggleButtonResponse, new InnerResponse { Response = _response.Response });
    }

    /// <inheritdoc cref="HeaderResponse.SetOpen"/>
    public void SetOpen(bool open)
    {
        _state.SetOpen(open);
    }

    /// <inheritdoc cref="HeaderResponse.Toggle"/>
    public void Toggle()
    {
        _state.Toggle(_ui);
    }

    /// <inheritdoc cref="HeaderResponse.Body"/>
    public (Response, InnerResponse<H>, InnerResponse?) Body(Action<Ui> addBody)
    {
        var bodyResponse = _state.ShowBodyIndented(_response.Response, _ui, addBody);
        return (_toggleButtonResponse, _response, bodyResponse);
    }

    /// <inheritdoc cref="HeaderResponse.Body"/>
    public (Response, InnerResponse<H>, InnerResponse<R>?) Body<R>(Func<Ui, R> addBody)
    {
        var bodyResponse = _state.ShowBodyIndented(_response.Response, _ui, addBody);
        return (_toggleButtonResponse, _response, bodyResponse);
    }

    /// <inheritdoc cref="HeaderResponse.Body"/>
    public (Response, InnerResponse<H>, InnerResponse?) BodyUnindented(Action<Ui> addBody)
    {
        var bodyResponse = _state.ShowBodyUnindented(_ui, addBody);
        return (_toggleButtonResponse, _response, bodyResponse);
    }

    /// <inheritdoc cref="HeaderResponse.Body"/>
    public (Response, InnerResponse<H>, InnerResponse<R>?) BodyUnindented<R>(Func<Ui, R> addBody)
    {
        var bodyResponse = _state.ShowBodyUnindented(_ui, addBody);
        return (_toggleButtonResponse, _response, bodyResponse);
    }
}