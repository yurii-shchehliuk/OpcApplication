import { NodeClass } from './enums';

export interface TreeNode {
  startNodeId: string;
  displayName: string;
  nodeClass: NodeClass;
  children: TreeNode[];
}
