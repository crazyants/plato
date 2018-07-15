namespace Plato.Internal.Scripting.Abstractions
{
    public interface IScriptManager
    {
        ScriptBlocks GetScriptBlocks(ScriptSection section);

        void SetScriptBlock(ScriptBlock block, ScriptSection section);

    }
    
}
