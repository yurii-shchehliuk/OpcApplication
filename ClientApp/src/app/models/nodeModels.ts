export interface NodeValue {
  nodeId: string;
  displayName: string;
  range: number | null;
  mSecs: number | null;
  storeTime: string;
  value: number;
}

export interface NodeReference {
  nodeId: string;
  displayName: string;
  subscriptionId: string | null;
  sessionEntityId: number;
  nodeClass: NodeClass;
  range: number | null;
  mSecs: number | null;
}

export interface TreeNode {
  nodeId: string;
  displayName: string;
  nodeClass: NodeClass;
  children: TreeNode[];
}

export interface EventData {
  message: string;
  title: string;
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
