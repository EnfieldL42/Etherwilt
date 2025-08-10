using UnityEngine;

[System.Serializable]
public class SerializableQuickSlotItem : ISerializationCallbackReceiver
{
    [SerializeField] public int itemID;
    [SerializeField] public int itemAmount;

    public QuickSlotItem GetQuickSlotItem()
    {
        QuickSlotItem item = WorldItemDatabase.instance.GetQuickSlotItemFromSerializedData(this);
        return item;
    }

    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {

    }
}
