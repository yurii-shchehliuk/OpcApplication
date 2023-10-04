export interface NodeData {
  nodeId: string;
  name: string;
  range: number | null;
  mSecs: number | null;
  group: string;
  storeTime: string;
  value: number;
}

export interface NodeBase {
  nodeId: string;
  name: string;
  browseName: string;
  displayName: string;
  sessionName: string;
  nodeClass: NodeClass;  
  range: number | null;
  mSecs: number | null;
  group: string;
}

export interface TreeNode {
  nodeId: string;
  name: string;
  browseName: string;
  displayName: string;
  nodeClass: NodeClass;
}

export enum NodeClass {
  Unspecified = 0,

  Object = 1,

  Variable = 2,

  Method = 4,

  ObjectType = 8,

  VariableType = 16,

  ReferenceType = 32,

  DataType = 64,

  View = 128,
}
