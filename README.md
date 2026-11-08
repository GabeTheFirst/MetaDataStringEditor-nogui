## Forked from:
- [MetadataStringEditor](https://github.com/JeremieCHN/MetaDataStringEditor)<br>

The goal here is to port MetadataStringEditor to dotnet 9 and allow it to work as a library in other applications.

## How to use:
Everything you need should be contained within MetaDataStringEditor
Create an editor with ``new Editor("EditorName")`` (The name doesn't matter, I just wanted to use it for logging)

Before you can do anything with an editor, you need to ``LoadFile("FilePath")``

After loading a file, you have access to the following:
``Editor.GetContains(string Text)`` // gets the first entry that contains the provided string
``Editor.GetEquals(string Text)`` // gets the first entry that equals the provided string 
``Editor.Save(bool CreateBackup = true)`` // optionally creates a backup, and saves over the opened file

Getting an entry will return an ``EditorListItem`` which you can edit.
Edit an entry with ``EditorListItem.SetNewStr(string NewValue)``
SetNewStr should update the main file as long as the class correctly obtained.

## Example
```cs
using MetaDataStringEditor;

namespace Example;

public static class ExampleProgram
{
    static void Main(string[] args)
    {
        Editor Metadata = new Editor("Example Editor");
        Metadata.LoadFile(".../path/to/file");
        EditorListItem Item = Metadata.GetContains("meow");
        Item.SetNewStr("Meowing");
        Metadata.Save(true);
    }
}
```