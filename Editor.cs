using LocalQuest;

namespace MetaDataStringEditor;

public class Editor(string InstanceName = "Metadata")
{
    MetadataFile? File;
    string Name = InstanceName;
    List<EditorListItem> Items = [];
    public void LoadFile(string FilePath)
    {
        Log.Info($"[{Name}] loading file...");
        File = new MetadataFile(FilePath); 
        Log.Info($"[{Name}] loading strings...");
        for(int i = 0; i < File.strBytes.Count; i++)
        {
            byte[] A = File.strBytes[i];
            Items.Add(new EditorListItem(A, File, i));
        }
        Log.Info($"[{Name}] Ready!");
    }

    bool CheckFileLoaded()
    {
        if(File == null)
        {
            Log.Warn("A file must be loaded before performing an action!");
            return false;
        }
        return true;
    }

    public EditorListItem? GetContains(string Key)
    {
        if(!CheckFileLoaded())
        {
            return null;
        }
        for(int i = 0; i < Items.Count; i++)
        {
            if(Items[i].MatchKeyWord(Key))
            {
                return Items[i];
            }
        }
        return null;
    }

    public EditorListItem? GetEquals(string Key)
    {
        if(!CheckFileLoaded())
        {
            return null;
        }
        for(int i = 0; i < Items.Count; i++)
        {
            if(Items[i].Text == Key)
            {
                return Items[i];
            }
        }
        return null;
    }

    public void Save(bool CreateBackup = true)
    {
        if(!CheckFileLoaded())
        {
            return;
        }
        if(CreateBackup)
        {
            File.WriteBackup($"{File.CurrentFileName}.bak");
        }
        File.WriteToNewFile(File.CurrentFileName);
    }
}