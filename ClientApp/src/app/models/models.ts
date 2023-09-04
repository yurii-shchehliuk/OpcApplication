//income node
export interface NodeData extends NodeBase {
  StoreTime: string;
  Value: number;
}

//outcome node
export interface NewNode extends NodeBase {
  Action: MonitorAction;
}

interface NodeBase {
  NodeId: string;
  Name: string;
  Range: number | null;
  MSecs: number | null;
  Group: string;
}

export enum MonitorAction {
  Monitor,
  Unpominotr,
}
