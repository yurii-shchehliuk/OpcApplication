import { Injectable } from '@angular/core';
import { NodeClass, TreeNode } from 'src/app/models/nodeModels';
import {
  CollectionViewer,
  SelectionChange,
  DataSource,
} from '@angular/cdk/collections';
import { BehaviorSubject, Observable, map, merge } from 'rxjs';
import { FlatTreeControl } from '@angular/cdk/tree';
import { TreeService } from 'src/app/services/tree.service';
import { NotificationService } from 'src/app/shared/notification.service';

/** Flat node with expandable and level information */
export class DynamicFlatNode {
  constructor(
    public item: TreeNode,
    public level = 1,
    public expandable = false,
    public isLoading = false
  ) {}
}

@Injectable({ providedIn: 'root' })
export class DynamicDatabase {
  childrenNodes: any;

  constructor(
    private treeService: TreeService,
    private notifyService: NotificationService,
  ) {}

  rootLevelNodes: TreeNode[] = [
    {
      nodeId: '',
      children: [],
      displayName: 'root',
      nodeClass: NodeClass.Object,
    },
  ];

  /** Initial data from database */
  initialData(): DynamicFlatNode[] {
    let t = this.rootLevelNodes.map(
      (treeNode) => new DynamicFlatNode(treeNode, 0, true)
    );
    return t;
  }

  getChildren(node: TreeNode) {
    return this.treeService.getTreeChildrens(node);
  }

  isExpandable(node: TreeNode): boolean {
    return node.children && node.children.length > 0;
  }
}
/**
 * File database, it can build a tree structured Json object from string.
 * Each node in Json object represents a file or a directory. For a file, it has filename and type.
 * For a directory, it has filename and children (a list of files or directories).
 * The input will be a json object string, and the output is a list of `FileNode` with nested
 * structure.
 */
export class DynamicDataSource implements DataSource<DynamicFlatNode> {
  dataChange = new BehaviorSubject<DynamicFlatNode[]>([]);

  get data(): DynamicFlatNode[] {
    return this.dataChange.value;
  }
  set data(value: DynamicFlatNode[]) {
    this._treeControl.dataNodes = value;
    this.dataChange.next(value);
  }

  constructor(
    private _treeControl: FlatTreeControl<DynamicFlatNode>,
    private _database: DynamicDatabase
  ) {}

  connect(collectionViewer: CollectionViewer): Observable<DynamicFlatNode[]> {
    this._treeControl.expansionModel.changed.subscribe((change) => {
      if (
        (change as SelectionChange<DynamicFlatNode>).added ||
        (change as SelectionChange<DynamicFlatNode>).removed
      ) {
        this.handleTreeControl(change as SelectionChange<DynamicFlatNode>);
      }
    });

    return merge(collectionViewer.viewChange, this.dataChange).pipe(
      map(() => this.data)
    );
  }

  disconnect(collectionViewer: CollectionViewer): void {}

  /** Handle expand/collapse behaviors */
  handleTreeControl(change: SelectionChange<DynamicFlatNode>) {
    if (change.added) {
      change.added.forEach((node) => this.toggleNode(node, true));
    }
    if (change.removed) {
      change.removed
        .slice()
        .reverse()
        .forEach((node) => this.toggleNode(node, false));
    }
  }

  /**
   * Toggle the node, remove from display list
   */
  toggleNode(node: DynamicFlatNode, expand: boolean) {
    const childrenObservable = this._database.getChildren(node.item);
    const index = this.data.indexOf(node);
    if (index < 0) {
      return;
    }

    node.isLoading = true;

    childrenObservable.subscribe({
      next: (children) => {
        if (expand) {
          const nodes = children.map(
            (treeNode) =>
              new DynamicFlatNode(
                treeNode,
                node.level + 1,
                this._database.isExpandable(treeNode)
              )
          );
          this.data.splice(index + 1, 0, ...nodes);
        } else {
          let count = 0;
          for (
            let i = index + 1;
            i < this.data.length && this.data[i].level > node.level;
            i++, count++
          ) {}
          this.data.splice(index + 1, count);
        }

        this.dataChange.next(this.data);
        node.isLoading = false;
      },
      error: (error) => {
        console.error('Error fetching children:', error);
        node.isLoading = false;
        this.dataChange.next(this._database.initialData());
      },
      complete: () => {
        node.isLoading = false;
      },
    });
  }
}
