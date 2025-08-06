[System.Serializable]
public class PlayerData {
    public string name;
    public bool isActive;

    public PlayerData(string name, bool isActive) {
        this.name = name;
        this.isActive = isActive;
    }
}