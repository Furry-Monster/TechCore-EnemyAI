# 复合节点 (Composite Nodes)

# 装饰器节点 (Decorator Nodes)

- Cooldown - 冷却器（限制执行频率）
- Timeout - 超时器（限制执行时间）败

# 动作节点 (Action Nodes)

## 移动相关

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

- SendMessage - 发送消息/事件

# 条件节点 (Condition Nodes)

## 视觉检测

- CanSeeTarget - 能看到目标
- IsTargetVisible - 目标可见
- LineOfSight - 视线检查

## 状态检测

- IsHealthLow - 血量低
- IsTargetDead - 目标死亡
- HasAmmo - 有弹药
- IsGrounded - 在地面上

## 时间条件

- TimeElapsed - 时间已过
- RandomChance - 随机概率
- Cooldown - 冷却完成

# 感知节点 (Perception Nodes)

- DetectEnemies - 检测敌人
- DetectSound - 检测声音
- ScanArea - 扫描区域
- UpdateTargetList - 更新目标列表