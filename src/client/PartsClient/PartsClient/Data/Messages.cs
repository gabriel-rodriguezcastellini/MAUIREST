using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PartsClient.Data;

public class RefreshMessage(bool value) : ValueChangedMessage<bool>(value)
{
}
