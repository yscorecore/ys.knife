using System;
using System.Collections.Generic;
using System.Text;

namespace System.Record
{
    
    public class OperationManager:IUndoRedo ,IUndoRedoRecordProvider
    {
        #region 成员及枚举
        enum LastState
        {
            None,  //上一次无操作
            Undo, //上一次执行的是UnDo
            Redo  //上一次执行的是Redo
        }
        private List<OperationBase> m_lst; //记录操作集合
        private int m_MaxCount;
        private int m_index = -1; //指针
        private LastState m_LastState; //上一次执行的是撤销,重复
        #endregion

        #region 事件
        /// <summary>
        /// 当栈达到最大数量或者是执行UnDo后在再进行了压栈从而覆盖了
        /// 后面的操作，开始移除不需要的操作时
        /// </summary>
        public event EventHandler<RecordEventArgs> ClearItem;
        /// <summary>
        /// 执行了一次Push操作之后
        /// </summary>
        public event EventHandler<RecordEventArgs> Pushed;
        /// <summary>
        /// 执行撤销操作之前
        /// </summary>
        public event EventHandler<RecordEventArgs> Undoing;
        /// <summary>
        /// 执行撤销操作之后
        /// </summary>
        public event EventHandler<RecordEventArgs> Undone;
        /// <summary>
        /// 执行重复操作之前
        /// </summary>
        public event EventHandler<RecordEventArgs> Redoing;
        /// <summary>
        /// 执行重复操作之后
        /// </summary>
        public event EventHandler<RecordEventArgs> Redone;

        #endregion

        #region 构造函数
        /// <summary>
        /// 默认最大记录数量为20
        /// </summary>
        public OperationManager ()
            : this(20)
        {

        }
        /// <summary>
        /// 默认最大记录数量为20
        /// </summary>
        /// <param name="maxcount">最大的记录数量</param>
        public OperationManager (int maxcount)
        {
            this.m_lst = new List<OperationBase>();
            this.m_MaxCount = maxcount;

        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否可以进行撤销
        /// </summary>
        public Boolean CanUndo
        {
            get
            {
                return this.RecordCount != 0 && this.m_index >= 0;
            }
        }
        /// <summary>
        /// 是否可以进行重复操作
        /// </summary>
        public Boolean CanRedo
        {
            get
            {
                if (this.m_LastState == OperationManager.LastState.None) return false;
                else return this.RecordCount != 0 && this.m_index <= this.m_lst.Count - 1;
            }
        }
        /// <summary>
        /// 当前记录的数量
        /// </summary>
        public int RecordCount
        {
            get { return this.m_lst.Count; }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 复位，即把记录请空
        /// </summary>
        public void Reset ()
        {
            OperationBase col;
            while (this.m_lst.Count != 0)
            {
                col = this.m_lst[0];
                this.m_lst.Remove(col);
                RecordEventArgs arg = new RecordEventArgs(col);
                OnClearItem(arg);
            }
            this.m_LastState = LastState.None;
            this.m_index = -1;
        }
        /// <summary>
        /// 插入一次操作记录
        /// </summary>
        /// <param name="op"></param>
        public void Push (OperationBase op)
        {
            //当进行了恢复操作，在进行插入时，则会截断后面的记录
            if (this.m_index < this.m_lst.Count - 1)
            {
                int tempindex = m_index < 0 ? 0 : this.m_index + 1;
                OperationBase col;
                while (tempindex < this.RecordCount)
                {
                    col = this.m_lst[tempindex];
                    this.m_lst.Remove(col);
                    RecordEventArgs arg = new RecordEventArgs(col);
                    this.OnClearItem(arg);
                }
            }
            if (this.RecordCount == this.m_MaxCount) //已经装满，则删除首个元素
            {
                OperationBase col = this.m_lst[0];
                this.m_lst.Remove(col);
                RecordEventArgs arg = new RecordEventArgs(col);
                this.OnClearItem(arg);
                this.m_lst.Add(op);
                this.m_index++;
            }
            else //还没有装满
            {
                this.m_lst.Add(op);
                //设置指针
                this.m_index++;
            }

            this.m_LastState = LastState.None;
            //激发事件
            OnPushed(new RecordEventArgs(op));
        }
        /// <summary>
        /// 插入一组操作。
        /// </summary>
        /// <param name="operations">操作的数组</param>
        public void Push (params OperationBase[] operations)
        {
            OperationBase group = new OperationGroup(operations);
            this.Push(group);
        }
        /// <summary>
        /// 插入一组操作。
        /// </summary>
        /// <param name="operations">操作集</param>
        public void Push (IEnumerable<OperationBase> operations)
        {
            OperationBase group = new OperationGroup(operations);
            this.Push(group);
        }

        /// <summary>
        /// 执行一次回退操作
        /// </summary>
        public void Undo ()
        {
            //如果上一次执行的是重复操作，则
            //指针将应该先下移一位
            if (this.m_LastState == LastState.Redo)
            {
                this.m_index--;
            }
            if (this.m_index >= this.RecordCount)
            {
                //指针越界时把指针移动到最上面位置
                this.m_index = this.RecordCount - 1;
            }

            if (this.CanUndo)
            {
                RecordEventArgs arg = new RecordEventArgs(this.m_lst[m_index]);
                OnUndoing(arg);
                this.m_lst[m_index].SetOldValue();
                //激发事件
                OnUndone(arg);
                this.m_LastState = LastState.Undo;
                this.m_index--;
            }
        }
        /// <summary>
        /// 执行一次恢复操作。
        /// </summary>
        public void Redo ()
        {
            //如果上一次执行的是撤销操作，则应该先把指针前移一位
            //否则，则会失去响应一次操作
            if (this.m_LastState == LastState.Undo)
            {
                this.m_index++;
            }
            if (this.m_index < 0 && this.RecordCount > 0)
            {
                //指针为负值，并且记录数大于0时把指针置于0位置
                this.m_index = 0;
            }
            if (this.CanRedo)
            {
                RecordEventArgs args=new RecordEventArgs (this.m_lst[m_index]);
                OnRedoing(args);
                this.m_lst[m_index].SetNewValue();
                OnRedone(args);
                //设置标记
                this.m_LastState = LastState.Redo;
                this.m_index++;
            }
        }
        /// <summary>
        /// 激发<see cref="ClearItem"/>事件。
        /// </summary>
        /// <param name="e">事件参数</param>
        protected virtual void OnClearItem (RecordEventArgs e)
        {
            if (this.ClearItem != null)
            {
                this.ClearItem(this, e);
            }
        }
        /// <summary>
        /// 激发<see cref="Pushed"/>事件。
        /// </summary>
        /// <param name="e">事件参数</param>
        protected virtual void OnPushed (RecordEventArgs e)
        {
            if (this.Pushed != null)
            {
                this.Pushed(this, e);
            }
        }
        /// <summary>
        /// 激发<see cref="Undone"/>事件。
        /// </summary>
        /// <param name="e">事件参数</param>
        private void OnUndone (RecordEventArgs e)
        {
            if (this.Undone != null)
            {
                this.Undone(this,e);
            }
        }
        /// <summary>
        /// 激发<see cref="Undoing"/>事件。
        /// </summary>
        /// <param name="e">事件参数</param>
        protected virtual void OnUndoing (RecordEventArgs e)
        {
            if (this.Undoing != null)
            {
                this.Undoing(this,e);
            }
        }
        /// <summary>
        /// 激发<see cref="Redone"/>事件。
        /// </summary>
        /// <param name="e">事件参数</param>
        private void OnRedone (RecordEventArgs e)
        {
            if (this.Redone != null)
            {
                this.Redone(this,e);
            }
        }
        /// <summary>
        /// 激发<see cref="Redoing"/>事件。
        /// </summary>
        /// <param name="e">事件参数</param>
        private void OnRedoing (RecordEventArgs e)
        {
            if (this.Redoing != null)
            {
                this.Redoing(this, e);
            }
        }
        #endregion

    }
   
}
