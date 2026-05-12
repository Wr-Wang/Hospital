# 核心业务流程（初版）

与需求清单 §3–§11 对齐，供架构设计与集成测试用例编写。图中节点采用 camelCase。

---

## 1. 预约到诊门诊闭环

```mermaid
flowchart LR
  subgraph booking [预约与号源]
    A1[维护号源]
    A2[多渠道预约]
    A3[规则校验]
  end
  subgraph onsite [到院]
    B1[预约取号]
    B2[现场挂号]
    B3[分诊入队]
  end
  subgraph clinic [诊间]
    C1[叫号接诊]
    C2[病历诊断]
    C3[开申请单与处方]
  end
  subgraph settle [结算与发药]
    D1[计价收费]
    D2[医保结算]
    D3[药房发药]
  end
  A1 --> A2 --> A3 --> B1
  A1 --> B2
  B1 --> B3
  B2 --> B3
  B3 --> C1 --> C2 --> C3 --> D1 --> D2 --> D3
```

---

## 2. 住院入出转与医嘱护理

```mermaid
flowchart TD
  H1[空床查询] --> H2[入院登记]
  H2 --> H3[预交金]
  H3 --> H4[医嘱开立]
  H4 --> H5[护士校对执行]
  H5 --> H6[计费记账]
  H4 --> H7[检验检查申请]
  H7 --> H8[医技执行与报告]
  H5 --> H9[生命体征与护理记录]
  H9 --> H10[监护报警闭环]
  H6 --> H11[出院预结算]
  H11 --> H12[出院结算与发票]
```

---

## 3. 药品：处方到发药与退药

```mermaid
sequenceDiagram
  participant Doc as 门诊医生
  participant Rx as 处方服务
  participant Inv as 库存
  participant Phar as 药师
  participant Pat as 患者/收费
  Doc->>Rx: 开立处方
  Rx->>Rx: 合理用药与权限校验
  Rx->>Inv: 锁库存或预留
  Pat->>Rx: 缴费完成
  Phar->>Inv: 扣减批号效期出库
  Phar->>Pat: 发药核对
  Pat->>Rx: 退药申请
  Rx->>Inv: 回库与冲账
```

---

## 4. 设备：报修到关闭

```mermaid
stateDiagram-v2
  [*] --> InUse
  InUse --> FaultReported: 报修
  FaultReported --> Dispatched: 派工
  Dispatched --> InRepair: 维修中
  InRepair --> Calibrated: 需计量/校准
  Calibrated --> InUse: 验收合格
  InRepair --> InUse: 直接验收
  InUse --> Retired: 报废审批
  Retired --> [*]
```

---

## 5. 危急值与监护报警（统一闭环）

```mermaid
flowchart LR
  W1[来源_LIS_PACS_监护仪] --> W2[规则引擎分级]
  W2 --> W3[推送医护]
  W3 --> W4[接收确认]
  W4 --> W5[处置记录]
  W5 --> W6[超时升级]
```

---

## 6. 多院区数据域（概念）

```mermaid
flowchart TB
  subgraph platform [统一平台]
    U[用户与角色]
    O[机构院区主数据]
  end
  U -->|scoped_by| O
  subgraph data [业务数据]
    P[患者EMPI全局]
    Q[就诊订单院区级]
    R[库存设备院区级]
  end
  O --> Q
  O --> R
  P --> Q
```

---

## 修订记录

| 版本 | 说明 |
|------|------|
| 1.0 | 与需求清单初版对齐 |
