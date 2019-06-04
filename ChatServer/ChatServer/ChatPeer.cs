using System;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System;

public class ChatPeer : ClientPeer
{
    public ChatPeer(InitRequest request)
        : base(request)
    {
        BroadcastMessage += OnBroadcastMessage;
    }

    private static event Action<ChatPeer, EventData, SendParameters> BroadcastMessage;

    protected override void OnDisconnect(DisconnectReason disconnectCode, string reasonDetail)
    {
        BroadcastMessage -= OnBroadcastMessage;
    }

    protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
    {
        if (operationRequest.OperationCode == 1) // Chat Custom Operation Code = 1
        {
            // broadcast chat custom event to other peers
            var eventData = new EventData(1) { Parameters = operationRequest.Parameters }; // Chat Custom Event Code = 1
            BroadcastMessage(this, eventData, sendParameters);
            // send operation response (~ACK) back to peer
            var response = new OperationResponse(operationRequest.OperationCode);
            SendOperationResponse(response, sendParameters);
        }
    }

    private void OnBroadcastMessage(ChatPeer peer, EventData eventData, SendParameters sendParameters)
    {
        if (peer != this) // do not send chat custom event to peer who called the chat custom operation
        {
            SendEvent(eventData, sendParameters);
        }
    }
}