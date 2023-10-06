import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl, NestedTreeControl } from '@angular/cdk/tree';
import {
  ChangeDetectorRef,
  Component,
  Injectable,
  NgZone,
  OnInit,
} from '@angular/core';
import {
  MatTreeFlatDataSource,
  MatTreeFlattener,
  MatTreeNestedDataSource,
} from '@angular/material/tree';
import { BehaviorSubject, forkJoin, tap } from 'rxjs';
import { TreeService } from '../../services/tree.service';
import { SessionService } from '../../services/session.service';
import { NodeClass, TreeNode } from 'src/app/models/nodeModels';

@Component({
  selector: 'app-node-tree',
  templateUrl: './node-tree.component.html',
  styleUrls: ['./node-tree.component.scss'],
})
export class NodeTreeComponent implements OnInit {
  treeControl = new NestedTreeControl<TreeNode>((node) => node.children);
  dataSource = new MatTreeNestedDataSource<TreeNode>();

  constructor(
    private treeService: TreeService,
    private changeDetector: ChangeDetectorRef,
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    this.fetchRootNodes();
  }

  fetchRootNodes(): void {
    this.treeService
      .getTreeChildrens({
        nodeId: '',
        displayName: 'root',
        nodeClass: NodeClass.Object,
        children: [],
      } as TreeNode)
      .subscribe((nodes: TreeNode[]) => {
        this.dataSource.data = nodes;
      });
  }

  // loadChildren(nodeParent: TreeNode): void {
  //   // If nodeParent does not have children array, create it to avoid errors.
  //   if (!nodeParent.children) {
  //     nodeParent.children = [];
  //   }
  //   // Storing the observables for fetching children
  //   const childrenFetchObservables = nodeParent.children.map(
  //     (childNode, index) => {
  //       return this.treeService.getTreeChildrens(childNode).pipe(
  //         tap((fetchedChild) => {
  //           // Update the actual children of nodeParent
  //           if (fetchedChild) {
  //             nodeParent.children[index] = fetchedChild[0];
  //           }
  //         })
  //       );
  //     }
  //   );

  //   // Using forkJoin to wait for all children fetches to be complete
  //   forkJoin(childrenFetchObservables).subscribe(
  //     () => {
  //       // Trigger a re-render of the tree by reassigning to dataSource.data
  //       this.ngZone.run(() => {
  //         // Ensuring this runs inside Angular's zone
  //         this.dataSource.data = [...this.dataSource.data];
  //       });
  //       this.changeDetector.detectChanges(); // Manually check the component and its children for changes
  //     },
  //     (error) => {
  //       console.error('Error fetching children: ', error);
  //       // Handle the error e.g. show a user-friendly error message
  //     }
  //   );
  // }

  loadChildren(nodeParent: TreeNode): void {
    nodeParent.children.map((node) => {
      this.treeService
        .getTreeChildrens(node)
        .subscribe((children: TreeNode[]) => {
          node = children[0];
          this.changeDetector.detectChanges();
        });
    });
  }

  hasChild = (_: number, node: TreeNode) =>
    !!node.children && node.children.length > 0;
}
