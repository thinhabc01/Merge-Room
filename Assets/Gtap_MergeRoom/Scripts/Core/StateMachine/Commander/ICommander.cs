using System;

public interface ICommander
{ 
    event Action<ICommand> CommandReceived; 
    void Command(ICommand command);
}