[System.Serializable]
public class PlayerInfo
{
    public string Name { get; private set; }
    public string Level { get; private set; }
    public MoveElements MoveStatus;
    public AttackElements AttackStatus;
    public Parameters Parameters;
}
