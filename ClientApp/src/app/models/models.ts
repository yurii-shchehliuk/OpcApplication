//income node
export interface NodeData extends NodeBase {
  storeTime: string;
  value: number;
}

//outcome node
export interface NewNode extends NodeBase {
  Action: MonitorAction;
}

interface NodeBase {
  nodeId: string;
  name: string;
  range: number | null;
  mSecs: number | null;
  group: string;
}

export interface TreeNode {
  nodeId: string;
  name: string;
}

export enum MonitorAction {
  Monitor,
  Unmomonitor,
}
