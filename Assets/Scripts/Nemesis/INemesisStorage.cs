using System.Collections.Generic;

public interface INemesisStorage
{
    Dictionary<string, int> LoadAll();                 // npcId -> level
    void SaveAll(Dictionary<string, int> levels);
}
