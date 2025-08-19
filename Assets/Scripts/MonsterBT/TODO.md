# 复合节点 (Composite Nodes)

- ✅ Selector - 选择器（或节点）
- ✅ Sequence - 序列器（与节点）
- ✅ Parallel - 并行节点（同时执行多个子节点）
- RandomSelector - 随机选择器
- WeightedSelector - 权重选择器
- PrioritySelector - 优先级选择器

# 装饰器节点 (Decorator Nodes)

- ✅ Inverter - 反转器
- ✅ Repeater - 重复器（重复N次或无限重复）
- Cooldown - 冷却器（限制执行频率）
- Timeout - 超时器（限制执行时间）
- ✅ UntilSuccess - 直到成功
- ✅ UntilFailure - 直到失败
- ✅ ForceSuccess - 强制成功
- ✅ ForceFailure - 强制失败

# 动作节点 (Action Nodes)

## 移动相关

- ✅ MoveToTarget - 移动到目标
- Patrol - 巡逻
- Wander - 游荡
- Chase - 追逐
- Flee - 逃离
- MoveToPosition - 移动到位置

## 动画相关

- PlayAnimation - 播放动画
- SetAnimationTrigger - 设置动画触发器
- WaitForAnimationComplete - 等待动画完成

## 战斗相关

- Attack - 攻击
- Block - 格挡
- UseSkill - 使用技能
- Reload - 重新装填

## 交互相关

- Interact - 与对象交互
- PickupItem - 拾取物品
- DropItem - 丢弃物品
- UseItem - 使用物品

## 通用相关

- ✅ Wait - 等待
- ✅ DebugLog - 调试日志
- SetBlackboardValue - 设置黑板值
- SendMessage - 发送消息/事件

# 条件节点 (Condition Nodes)

## 距离检测

- ✅ DistanceCondition - 距离条件
- InRange - 在范围内
- OutOfRange - 超出范围

## 视觉检测

- CanSeeTarget - 能看到目标
- IsTargetVisible - 目标可见
- LineOfSight - 视线检查

## 状态检测

- IsHealthLow - 血量低
- IsTargetDead - 目标死亡
- HasAmmo - 有弹药
- IsGrounded - 在地面上

## 黑板条件

- BlackboardBool - 黑板布尔值检查
- BlackboardFloat - 黑板浮点值比较
- BlackboardObject - 黑板对象检查

## 时间条件

- TimeElapsed - 时间已过
- RandomChance - 随机概率
- Cooldown - 冷却完成

# 感知节点 (Perception Nodes)

- DetectEnemies - 检测敌人
- DetectSound - 检测声音
- ScanArea - 扫描区域
- UpdateTargetList - 更新目标列表

高优先级（核心功能）：

- Parallel, RandomSelector
- Repeater, Cooldown, Timeout
- Patrol, Chase, Flee
- PlayAnimation, Attack
- CanSeeTarget, IsHealthLow, BlackboardBool

中优先级（增强功能）：

- WeightedSelector, PrioritySelector
- UntilSuccess, ForceSuccess
- Wander, UseSkill
- SetBlackboardValue, SendMessage
- RandomChance, TimeElapsed

低优先级（高级功能）：

- 复杂的感知节点
- 高级交互节点
- 特殊用途装饰器
