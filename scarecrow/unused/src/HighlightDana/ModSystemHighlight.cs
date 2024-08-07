using System.Threading;

namespace scarecrow.unused.src.HighlightDana;

public class ModSystemHighlight : ModSystem, IThreadHighlight
{
    public bool Enabled { get; set; }
    public Thread OpThread { get; set; }
    public virtual string ThreadName { get; }

    public virtual string Name { get; }
    public virtual string HotkeyCode { get; }

    public bool ToggleRun(ICoreClientAPI capi)
    {
        if (!Enabled)
        {
            Enabled = true;
            OpThread = new Thread(() => Run(capi))
            {
                IsBackground = true,
                Name = ThreadName
            };
            OpThread.Start();
        }
        else
        {
            Enabled = false;
        }

        capi.TriggerChatMessage(Constants.StringToggle(Enabled, Name));

        return true;
    }

    protected void Run(ICoreClientAPI capi)
    {
        while (Enabled)
        {
            Thread.Sleep(100);
            try
            {
                OnRunning(capi);
            }
            catch (ThreadAbortException)
            {

#pragma warning disable SYSLIB0006 // Typ oder Element ist veraltet
                Thread.ResetAbort();
#pragma warning restore SYSLIB0006 // Typ oder Element ist veraltet
                break;
            }
            catch { }
        }
        ClearHighlights(capi);
    }

    public virtual void OnRunning(ICoreClientAPI capi)
    {
    }

    private void ClearHighlights(ICoreClientAPI capi)
    {
        capi.Event.EnqueueMainThreadTask(new Action(() => capi.World.HighlightBlocks(capi.World.Player, 5229, new List<BlockPos>())), ThreadName);
    }
}