# Unity Mission System
![Core UML](./CoreUML.png)

## MissionRequire

---

### MissionRequireTemplate

```csharp
public abstract class MissionRequireTemplate : MissionRequire<object>
{
    public abstract class MissionRequireTemplateHandle : MissionRequireHandle<object>
    {
        protected MissionRequireTemplateHandle(MissionRequireTemplate require) : base(require) { }
    }
}
```

MissionRequireTemplate 是具体任务需求的基类

```
public class CommonMissionRequire : MissionRequireTemplate
{
    [SerializeField] private GameEventType type;
    [SerializeField] private string args;
    [SerializeField] private int count;

    public CommonMissionRequire()
    {
        type = GameEventType.Default;
        args = null;
        count = 0;
    }

    public CommonMissionRequire(GameEventType type, string args, int count)
    {
        this.type = type;
        this.args = args;
        this.count = count;
    }

    public override bool CheckMessage(object message)
    {
        if (message is not GameMessage gameMessage) return false;
        return gameMessage.type == type && gameMessage.args?.ToString() == args;
    }

    [System.Serializable]
    public class Handle : MissionRequireTemplateHandle
    {
        private readonly CommonMissionRequire require;
        private int count;

        public Handle(CommonMissionRequire gmRequire) : base(gmRequire)
        {
            require = gmRequire;
        }

        protected override bool UseMessage(object message)
        {
            return ++count >= require.count;
        }
    }
}
```

CommonMissionRequire 是基类的一种实现，监听GameMessage类型消息。若需要扩展其他任务需求，请注意不要改变内部Handle类名！因为MissionRequire的CreateHandle函数会通过反射创建MissionRequireHandle。



**Q1. 为什么把 T 固化成 object，而不是 GameMessage(某种具体的消息类型)？**
A：若条件需要监听多种消息类型，object 是最低阻力方案。



参考文章：
[可拓展性开放世界轻量级任务系统框架实现(基于Unity+NodeCanvas)](https://zhuanlan.zhihu.com/p/805905472)

