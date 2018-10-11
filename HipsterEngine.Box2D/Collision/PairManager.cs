// Decompiled with JetBrains decompiler
// Type: Box2DX.Collision.PairManager
// Assembly: Box2DX, Version=2.0.1.28279, Culture=neutral, PublicKeyToken=null
// MVID: BD116DE6-476A-4E37-B8FC-78B19E349C03
// Assembly location: /Users/kusy/Documents/Новая папка/HipsterEngine/bin/Debug/Box2DX.dll

using Box2DX.Common;
using System;

namespace Box2DX.Collision
{
  public class PairManager
  {
    public static readonly ushort NullPair = Box2DX.Common.Math.USHRT_MAX;
    public static readonly ushort NullProxy = Box2DX.Common.Math.USHRT_MAX;
    public static readonly int TableCapacity = Settings.MaxPairs;
    public static readonly int TableMask = PairManager.TableCapacity - 1;
    public Pair[] _pairs = new Pair[Settings.MaxPairs];
    public BufferedPair[] _pairBuffer = new BufferedPair[Settings.MaxPairs];
    public ushort[] _hashTable = new ushort[PairManager.TableCapacity];
    public BroadPhase _broadPhase;
    public PairCallback _callback;
    public ushort _freePair;
    public int _pairCount;
    public int _pairBufferCount;

    public PairManager()
    {
      Box2DXDebug.Assert(Box2DX.Common.Math.IsPowerOfTwo((uint) PairManager.TableCapacity));
      Box2DXDebug.Assert(PairManager.TableCapacity >= Settings.MaxPairs);
      for (int index = 0; index < PairManager.TableCapacity; ++index)
        this._hashTable[index] = PairManager.NullPair;
      this._freePair = (ushort) 0;
      for (int index = 0; index < Settings.MaxPairs; ++index)
      {
        this._pairs[index] = new Pair();
        this._pairs[index].ProxyId1 = PairManager.NullProxy;
        this._pairs[index].ProxyId2 = PairManager.NullProxy;
        this._pairs[index].UserData = (object) null;
        this._pairs[index].Status = (Pair.PairStatus) 0;
        this._pairs[index].Next = (ushort) (index + 1);
      }
      this._pairs[Settings.MaxPairs - 1].Next = PairManager.NullPair;
      this._pairCount = 0;
      this._pairBufferCount = 0;
    }

    public void Initialize(BroadPhase broadPhase, PairCallback callback)
    {
      this._broadPhase = broadPhase;
      this._callback = callback;
    }

    public void AddBufferedPair(int id1, int id2)
    {
      Box2DXDebug.Assert(id1 != (int) PairManager.NullProxy && id2 != (int) PairManager.NullProxy);
      Box2DXDebug.Assert(this._pairBufferCount < Settings.MaxPairs);
      Pair pair = this.AddPair(id1, id2);
      if (!pair.IsBuffered())
      {
        Box2DXDebug.Assert(!pair.IsFinal());
        pair.SetBuffered();
        this._pairBuffer[this._pairBufferCount].ProxyId1 = pair.ProxyId1;
        this._pairBuffer[this._pairBufferCount].ProxyId2 = pair.ProxyId2;
        ++this._pairBufferCount;
        Box2DXDebug.Assert(this._pairBufferCount <= this._pairCount);
      }
      pair.ClearRemoved();
      if (!BroadPhase.IsValidate)
        return;
      this.ValidateBuffer();
    }

    public void RemoveBufferedPair(int id1, int id2)
    {
      Box2DXDebug.Assert(id1 != (int) PairManager.NullProxy && id2 != (int) PairManager.NullProxy);
      Box2DXDebug.Assert(this._pairBufferCount < Settings.MaxPairs);
      Pair pair = this.Find(id1, id2);
      if (pair == null)
        return;
      if (!pair.IsBuffered())
      {
        Box2DXDebug.Assert(pair.IsFinal());
        pair.SetBuffered();
        this._pairBuffer[this._pairBufferCount].ProxyId1 = pair.ProxyId1;
        this._pairBuffer[this._pairBufferCount].ProxyId2 = pair.ProxyId2;
        ++this._pairBufferCount;
        Box2DXDebug.Assert(this._pairBufferCount <= this._pairCount);
      }
      pair.SetRemoved();
      if (!BroadPhase.IsValidate)
        return;
      this.ValidateBuffer();
    }

    public void Commit()
    {
      int index1 = 0;
      Proxy[] proxyPool = this._broadPhase._proxyPool;
      for (int index2 = 0; index2 < this._pairBufferCount; ++index2)
      {
        Pair pair = this.Find((int) this._pairBuffer[index2].ProxyId1, (int) this._pairBuffer[index2].ProxyId2);
        Box2DXDebug.Assert(pair.IsBuffered());
        pair.ClearBuffered();
        Box2DXDebug.Assert((int) pair.ProxyId1 < Settings.MaxProxies && (int) pair.ProxyId2 < Settings.MaxProxies);
        Proxy p1 = proxyPool[(int) pair.ProxyId1];
        Proxy p2 = proxyPool[(int) pair.ProxyId2];
        Box2DXDebug.Assert(p1.IsValid);
        Box2DXDebug.Assert(p2.IsValid);
        if (pair.IsRemoved())
        {
          if (pair.IsFinal())
            this._callback.PairRemoved(p1.UserData, p2.UserData, pair.UserData);
          this._pairBuffer[index1].ProxyId1 = pair.ProxyId1;
          this._pairBuffer[index1].ProxyId2 = pair.ProxyId2;
          ++index1;
        }
        else
        {
          Box2DXDebug.Assert(this._broadPhase.TestOverlap(p1, p2));
          if (!pair.IsFinal())
          {
            pair.UserData = this._callback.PairAdded(p1.UserData, p2.UserData);
            pair.SetFinal();
          }
        }
      }
      for (int index2 = 0; index2 < index1; ++index2)
        this.RemovePair((int) this._pairBuffer[index2].ProxyId1, (int) this._pairBuffer[index2].ProxyId2);
      this._pairBufferCount = 0;
      if (!BroadPhase.IsValidate)
        return;
      this.ValidateTable();
    }

    private Pair Find(int proxyId1, int proxyId2)
    {
      if (proxyId1 > proxyId2)
        Box2DX.Common.Math.Swap<int>(ref proxyId1, ref proxyId2);
      uint hash = (uint) ((ulong) this.Hash((uint) proxyId1, (uint) proxyId2) & (ulong) PairManager.TableMask);
      return this.Find(proxyId1, proxyId2, hash);
    }

    private Pair Find(int proxyId1, int proxyId2, uint hash)
    {
      /// TODO: int next = (int) this._hashTable[(IntPtr) hash];
      
      int next = (int) this._hashTable[hash];
      while (next != (int) PairManager.NullPair && !this.Equals(this._pairs[next], proxyId1, proxyId2))
        next = (int) this._pairs[next].Next;
      if (next == (int) PairManager.NullPair)
        return (Pair) null;
      Box2DXDebug.Assert(next < Settings.MaxPairs);
      return this._pairs[next];
    }

    private Pair AddPair(int proxyId1, int proxyId2)
    {
      if (proxyId1 > proxyId2)
        Box2DX.Common.Math.Swap<int>(ref proxyId1, ref proxyId2);
      int index = (int) ((long) this.Hash((uint) proxyId1, (uint) proxyId2) & (long) PairManager.TableMask);
      Pair pair1 = this.Find(proxyId1, proxyId2, (uint) index);
      if (pair1 != null)
        return pair1;
      Box2DXDebug.Assert(this._pairCount < Settings.MaxPairs && (int) this._freePair != (int) PairManager.NullPair);
      ushort freePair = this._freePair;
      Pair pair2 = this._pairs[(int) freePair];
      this._freePair = pair2.Next;
      pair2.ProxyId1 = (ushort) proxyId1;
      pair2.ProxyId2 = (ushort) proxyId2;
      pair2.Status = (Pair.PairStatus) 0;
      pair2.UserData = (object) null;
      pair2.Next = this._hashTable[index];
      this._hashTable[index] = freePair;
      ++this._pairCount;
      return pair2;
    }

    private object RemovePair(int proxyId1, int proxyId2)
    {
      Box2DXDebug.Assert(this._pairCount > 0);
      if (proxyId1 > proxyId2)
        Box2DX.Common.Math.Swap<int>(ref proxyId1, ref proxyId2);
      int index1 = (int) ((long) this.Hash((uint) proxyId1, (uint) proxyId2) & (long) PairManager.TableMask);
      ushort next1 = this._hashTable[index1];
      bool flag = false;
      int index2 = 0;
      while ((int) next1 != (int) PairManager.NullPair)
      {
        if (this.Equals(this._pairs[(int) next1], proxyId1, proxyId2))
        {
          ushort num = next1;
          ushort next2 = this._pairs[(int) next1].Next;
          if (flag)
            this._pairs[index2].Next = next2;
          else
            this._hashTable[index1] = next2;
          Pair pair = this._pairs[(int) num];
          object userData = pair.UserData;
          pair.Next = this._freePair;
          pair.ProxyId1 = PairManager.NullProxy;
          pair.ProxyId2 = PairManager.NullProxy;
          pair.UserData = (object) null;
          pair.Status = (Pair.PairStatus) 0;
          this._freePair = num;
          --this._pairCount;
          return userData;
        }
        index2 = (int) next1;
        next1 = this._pairs[index2].Next;
        flag = true;
      }
      Box2DXDebug.Assert(false);
      return (object) null;
    }

    private void ValidateBuffer()
    {
      Box2DXDebug.Assert(this._pairBufferCount <= this._pairCount);
      BufferedPair[] array = new BufferedPair[this._pairBufferCount];
      Array.Copy((Array) this._pairBuffer, 0, (Array) array, 0, this._pairBufferCount);
      Array.Sort<BufferedPair>(array, new Comparison<BufferedPair>(PairManager.BufferedPairSortPredicate));
      Array.Copy((Array) array, 0, (Array) this._pairBuffer, 0, this._pairBufferCount);
      for (int index = 0; index < this._pairBufferCount; ++index)
      {
        if (index > 0)
          Box2DXDebug.Assert(!object.Equals((object) this._pairBuffer[index], (object) this._pairBuffer[index - 1]));
        Pair pair = this.Find((int) this._pairBuffer[index].ProxyId1, (int) this._pairBuffer[index].ProxyId2);
        Box2DXDebug.Assert(pair.IsBuffered());
        Box2DXDebug.Assert((int) pair.ProxyId1 != (int) pair.ProxyId2);
        Box2DXDebug.Assert((int) pair.ProxyId1 < Settings.MaxProxies);
        Box2DXDebug.Assert((int) pair.ProxyId2 < Settings.MaxProxies);
        Proxy proxy1 = this._broadPhase._proxyPool[(int) pair.ProxyId1];
        Proxy proxy2 = this._broadPhase._proxyPool[(int) pair.ProxyId2];
        Box2DXDebug.Assert(proxy1.IsValid);
        Box2DXDebug.Assert(proxy2.IsValid);
      }
    }

    private void ValidateTable()
    {
      Pair pair;
      for (int index = 0; index < PairManager.TableCapacity; ++index)
      {
        for (ushort next = this._hashTable[index]; (int) next != (int) PairManager.NullPair; next = pair.Next)
        {
          pair = this._pairs[(int) next];
          Box2DXDebug.Assert(!pair.IsBuffered());
          Box2DXDebug.Assert(pair.IsFinal());
          Box2DXDebug.Assert(!pair.IsRemoved());
          Box2DXDebug.Assert((int) pair.ProxyId1 != (int) pair.ProxyId2);
          Box2DXDebug.Assert((int) pair.ProxyId1 < Settings.MaxProxies);
          Box2DXDebug.Assert((int) pair.ProxyId2 < Settings.MaxProxies);
          Proxy p1 = this._broadPhase._proxyPool[(int) pair.ProxyId1];
          Proxy p2 = this._broadPhase._proxyPool[(int) pair.ProxyId2];
          Box2DXDebug.Assert(p1.IsValid);
          Box2DXDebug.Assert(p2.IsValid);
          Box2DXDebug.Assert(this._broadPhase.TestOverlap(p1, p2));
        }
      }
    }

    private uint Hash(uint proxyId1, uint proxyId2)
    {
      uint num1 = proxyId2 << 16 | proxyId1;
      uint num2 = (uint) (~(int) num1 + ((int) num1 << 15));
      uint num3 = num2 ^ num2 >> 12;
      uint num4 = num3 + (num3 << 2);
      uint num5 = (num4 ^ num4 >> 4) * 2057U;
      return num5 ^ num5 >> 16;
    }

    private bool Equals(Pair pair, int proxyId1, int proxyId2)
    {
      return (int) pair.ProxyId1 == proxyId1 && (int) pair.ProxyId2 == proxyId2;
    }

    private bool Equals(ref BufferedPair pair1, ref BufferedPair pair2)
    {
      return (int) pair1.ProxyId1 == (int) pair2.ProxyId1 && (int) pair1.ProxyId2 == (int) pair2.ProxyId2;
    }

    public static int BufferedPairSortPredicate(BufferedPair pair1, BufferedPair pair2)
    {
      if ((int) pair1.ProxyId1 > (int) pair2.ProxyId1)
        return 1;
      if ((int) pair1.ProxyId1 < (int) pair2.ProxyId1)
        return -1;
      if ((int) pair1.ProxyId2 > (int) pair2.ProxyId2)
        return 1;
      return (int) pair1.ProxyId2 < (int) pair2.ProxyId2 ? -1 : 0;
    }
  }
}
