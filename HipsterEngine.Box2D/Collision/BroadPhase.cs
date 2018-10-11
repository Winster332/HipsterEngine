using Box2DX.Common;

namespace Box2DX.Collision
{
  public class BroadPhase
  {
    public static readonly ushort BROADPHASE_MAX = Math.USHRT_MAX;
    public static readonly ushort Invalid = BroadPhase.BROADPHASE_MAX;
    public static readonly ushort NullEdge = BroadPhase.BROADPHASE_MAX;
    public static bool IsValidate = false;
    public Proxy[] _proxyPool = new Proxy[Settings.MaxProxies];
    public Bound[][] _bounds = new Bound[2][];
    public ushort[] _queryResults = new ushort[Settings.MaxProxies];
    public float[] _querySortKeys = new float[Settings.MaxProxies];
    private int qi1 = 0;
    private int qi2 = 0;
    public PairManager _pairManager;
    public ushort _freeProxy;
    public int _queryResultCount;
    public AABB _worldAABB;
    public Vec2 _quantizationFactor;
    public int _proxyCount;
    public ushort _timeStamp;

    public BroadPhase(AABB worldAABB, PairCallback callback)
    {
      this._pairManager = new PairManager();
      this._pairManager.Initialize(this, callback);
      Box2DXDebug.Assert(worldAABB.IsValid);
      this._worldAABB = worldAABB;
      this._proxyCount = 0;
      Vec2 vec2 = worldAABB.UpperBound - worldAABB.LowerBound;
      this._quantizationFactor.X = (float) BroadPhase.BROADPHASE_MAX / vec2.X;
      this._quantizationFactor.Y = (float) BroadPhase.BROADPHASE_MAX / vec2.Y;
      for (ushort index = 0; (int) index < Settings.MaxProxies - 1; ++index)
      {
        this._proxyPool[(int) index] = new Proxy();
        this._proxyPool[(int) index].Next = (ushort) ((uint) index + 1U);
        this._proxyPool[(int) index].TimeStamp = (ushort) 0;
        this._proxyPool[(int) index].OverlapCount = BroadPhase.Invalid;
        this._proxyPool[(int) index].UserData = (object) null;
      }
      this._proxyPool[Settings.MaxProxies - 1] = new Proxy();
      this._proxyPool[Settings.MaxProxies - 1].Next = PairManager.NullProxy;
      this._proxyPool[Settings.MaxProxies - 1].TimeStamp = (ushort) 0;
      this._proxyPool[Settings.MaxProxies - 1].OverlapCount = BroadPhase.Invalid;
      this._proxyPool[Settings.MaxProxies - 1].UserData = (object) null;
      this._freeProxy = (ushort) 0;
      this._timeStamp = (ushort) 1;
      this._queryResultCount = 0;
      for (int index = 0; index < 2; ++index)
        this._bounds[index] = new Bound[2 * Settings.MaxProxies];
      int num = 2 * Settings.MaxProxies;
      for (int index1 = 0; index1 < 2; ++index1)
      {
        for (int index2 = 0; index2 < num; ++index2)
          this._bounds[index1][index2] = new Bound();
      }
    }

    public bool InRange(AABB aabb)
    {
      Vec2 vec2 = Math.Max(aabb.LowerBound - this._worldAABB.UpperBound, this._worldAABB.LowerBound - aabb.UpperBound);
      return (double) Math.Max(vec2.X, vec2.Y) < 0.0;
    }

    public ushort CreateProxy(AABB aabb, object userData)
    {
      Box2DXDebug.Assert(this._proxyCount < Settings.MaxProxies);
      Box2DXDebug.Assert((int) this._freeProxy != (int) PairManager.NullProxy);
      ushort freeProxy = this._freeProxy;
      Proxy proxy1 = this._proxyPool[(int) freeProxy];
      this._freeProxy = proxy1.Next;
      proxy1.OverlapCount = (ushort) 0;
      proxy1.UserData = userData;
      int boundCount = 2 * this._proxyCount;
      ushort[] lowerValues = new ushort[2];
      ushort[] upperValues = new ushort[2];
      this.ComputeBounds(out lowerValues, out upperValues, aabb);
      for (int axis = 0; axis < 2; ++axis)
      {
        Bound[] bound = this._bounds[axis];
        int lowerQueryOut;
        int upperQueryOut;
        this.Query(out lowerQueryOut, out upperQueryOut, lowerValues[axis], upperValues[axis], bound, boundCount, axis);
        Bound[] boundArray1 = new Bound[boundCount - upperQueryOut];
        for (int index = 0; index < boundCount - upperQueryOut; ++index)
          boundArray1[index] = bound[upperQueryOut + index].Clone();
        for (int index = 0; index < boundCount - upperQueryOut; ++index)
          bound[upperQueryOut + 2 + index] = boundArray1[index];
        Bound[] boundArray2 = new Bound[upperQueryOut - lowerQueryOut];
        for (int index = 0; index < upperQueryOut - lowerQueryOut; ++index)
          boundArray2[index] = bound[lowerQueryOut + index].Clone();
        for (int index = 0; index < upperQueryOut - lowerQueryOut; ++index)
          bound[lowerQueryOut + 1 + index] = boundArray2[index];
        ++upperQueryOut;
        bound[lowerQueryOut].Value = lowerValues[axis];
        bound[lowerQueryOut].ProxyId = freeProxy;
        bound[upperQueryOut].Value = upperValues[axis];
        bound[upperQueryOut].ProxyId = freeProxy;
        bound[lowerQueryOut].StabbingCount = lowerQueryOut == 0 ? (ushort) 0 : bound[lowerQueryOut - 1].StabbingCount;
        bound[upperQueryOut].StabbingCount = bound[upperQueryOut - 1].StabbingCount;
        for (int index = lowerQueryOut; index < upperQueryOut; ++index)
          ++bound[index].StabbingCount;
        for (int index = lowerQueryOut; index < boundCount + 2; ++index)
        {
          Proxy proxy2 = this._proxyPool[(int) bound[index].ProxyId];
          if (bound[index].IsLower)
            proxy2.LowerBounds[axis] = (ushort) index;
          else
            proxy2.UpperBounds[axis] = (ushort) index;
        }
      }
      ++this._proxyCount;
      Box2DXDebug.Assert(this._queryResultCount < Settings.MaxProxies);
      for (int index = 0; index < this._queryResultCount; ++index)
      {
        Box2DXDebug.Assert((int) this._queryResults[index] < Settings.MaxProxies);
        Box2DXDebug.Assert(this._proxyPool[(int) this._queryResults[index]].IsValid);
        this._pairManager.AddBufferedPair((int) freeProxy, (int) this._queryResults[index]);
      }
      this._pairManager.Commit();
      if (BroadPhase.IsValidate)
        this.Validate();
      this._queryResultCount = 0;
      this.IncrementTimeStamp();
      return freeProxy;
    }

    public void DestroyProxy(int proxyId)
    {
      Box2DXDebug.Assert(0 < this._proxyCount && this._proxyCount <= Settings.MaxProxies);
      Proxy proxy1 = this._proxyPool[proxyId];
      Box2DXDebug.Assert(proxy1.IsValid);
      int num = 2 * this._proxyCount;
      for (int axis = 0; axis < 2; ++axis)
      {
        Bound[] bound = this._bounds[axis];
        int lowerQueryOut = (int) proxy1.LowerBounds[axis];
        int upperQueryOut = (int) proxy1.UpperBounds[axis];
        ushort lowerValue = bound[lowerQueryOut].Value;
        ushort upperValue = bound[upperQueryOut].Value;
        Bound[] boundArray1 = new Bound[upperQueryOut - lowerQueryOut - 1];
        for (int index = 0; index < upperQueryOut - lowerQueryOut - 1; ++index)
          boundArray1[index] = bound[lowerQueryOut + 1 + index].Clone();
        for (int index = 0; index < upperQueryOut - lowerQueryOut - 1; ++index)
          bound[lowerQueryOut + index] = boundArray1[index];
        Bound[] boundArray2 = new Bound[num - upperQueryOut - 1];
        for (int index = 0; index < num - upperQueryOut - 1; ++index)
          boundArray2[index] = bound[upperQueryOut + 1 + index].Clone();
        for (int index = 0; index < num - upperQueryOut - 1; ++index)
          bound[upperQueryOut - 1 + index] = boundArray2[index];
        for (int index = lowerQueryOut; index < num - 2; ++index)
        {
          Proxy proxy2 = this._proxyPool[(int) bound[index].ProxyId];
          if (bound[index].IsLower)
            proxy2.LowerBounds[axis] = (ushort) index;
          else
            proxy2.UpperBounds[axis] = (ushort) index;
        }
        for (int index = lowerQueryOut; index < upperQueryOut - 1; ++index)
          --bound[index].StabbingCount;
        this.Query(out lowerQueryOut, out upperQueryOut, lowerValue, upperValue, bound, num - 2, axis);
      }
      Box2DXDebug.Assert(this._queryResultCount < Settings.MaxProxies);
      for (int index = 0; index < this._queryResultCount; ++index)
      {
        Box2DXDebug.Assert(this._proxyPool[(int) this._queryResults[index]].IsValid);
        this._pairManager.RemoveBufferedPair(proxyId, (int) this._queryResults[index]);
      }
      this._pairManager.Commit();
      this._queryResultCount = 0;
      this.IncrementTimeStamp();
      proxy1.UserData = (object) null;
      proxy1.OverlapCount = BroadPhase.Invalid;
      proxy1.LowerBounds[0] = BroadPhase.Invalid;
      proxy1.LowerBounds[1] = BroadPhase.Invalid;
      proxy1.UpperBounds[0] = BroadPhase.Invalid;
      proxy1.UpperBounds[1] = BroadPhase.Invalid;
      proxy1.Next = this._freeProxy;
      this._freeProxy = (ushort) proxyId;
      --this._proxyCount;
      if (!BroadPhase.IsValidate)
        return;
      this.Validate();
    }

    public void MoveProxy(int proxyId, AABB aabb)
    {
      if (proxyId == (int) PairManager.NullProxy || Settings.MaxProxies <= proxyId)
        Box2DXDebug.Assert(false);
      else if (!aabb.IsValid)
      {
        Box2DXDebug.Assert(false);
      }
      else
      {
        int num1 = 2 * this._proxyCount;
        Proxy proxy = this._proxyPool[proxyId];
        BoundValues b1 = new BoundValues();
        this.ComputeBounds(out b1.LowerValues, out b1.UpperValues, aabb);
        BoundValues b2 = new BoundValues();
        for (int index = 0; index < 2; ++index)
        {
          b2.LowerValues[index] = this._bounds[index][(int) proxy.LowerBounds[index]].Value;
          b2.UpperValues[index] = this._bounds[index][(int) proxy.UpperBounds[index]].Value;
        }
        for (int index1 = 0; index1 < 2; ++index1)
        {
          Bound[] bound1 = this._bounds[index1];
          int lowerBound = (int) proxy.LowerBounds[index1];
          int upperBound = (int) proxy.UpperBounds[index1];
          ushort lowerValue = b1.LowerValues[index1];
          ushort upperValue = b1.UpperValues[index1];
          int num2 = (int) lowerValue - (int) bound1[lowerBound].Value;
          int num3 = (int) upperValue - (int) bound1[upperBound].Value;
          bound1[lowerBound].Value = lowerValue;
          bound1[upperBound].Value = upperValue;
          if (num2 < 0)
          {
            for (int index2 = lowerBound; index2 > 0 && (int) lowerValue < (int) bound1[index2 - 1].Value; --index2)
            {
              Bound bound2 = bound1[index2];
              Bound bound3 = bound1[index2 - 1];
              int proxyId1 = (int) bound3.ProxyId;
              Proxy p = this._proxyPool[(int) bound3.ProxyId];
              ++bound3.StabbingCount;
              if (bound3.IsUpper)
              {
                if (this.TestOverlap(b1, p))
                  this._pairManager.AddBufferedPair(proxyId, proxyId1);
                ++p.UpperBounds[index1];
                ++bound2.StabbingCount;
              }
              else
              {
                ++p.LowerBounds[index1];
                --bound2.StabbingCount;
              }
              --proxy.LowerBounds[index1];
              Math.Swap<Bound>(ref bound1[index2], ref bound1[index2 - 1]);
            }
          }
          if (num3 > 0)
          {
            for (int index2 = upperBound; index2 < num1 - 1 && (int) bound1[index2 + 1].Value <= (int) upperValue; ++index2)
            {
              Bound bound2 = bound1[index2];
              Bound bound3 = bound1[index2 + 1];
              int proxyId1 = (int) bound3.ProxyId;
              Proxy p = this._proxyPool[proxyId1];
              ++bound3.StabbingCount;
              if (bound3.IsLower)
              {
                if (this.TestOverlap(b1, p))
                  this._pairManager.AddBufferedPair(proxyId, proxyId1);
                --p.LowerBounds[index1];
                ++bound2.StabbingCount;
              }
              else
              {
                --p.UpperBounds[index1];
                --bound2.StabbingCount;
              }
              ++proxy.UpperBounds[index1];
              Math.Swap<Bound>(ref bound1[index2], ref bound1[index2 + 1]);
            }
          }
          if (num2 > 0)
          {
            for (int index2 = lowerBound; index2 < num1 - 1 && (int) bound1[index2 + 1].Value <= (int) lowerValue; ++index2)
            {
              Bound bound2 = bound1[index2];
              Bound bound3 = bound1[index2 + 1];
              int proxyId1 = (int) bound3.ProxyId;
              Proxy p = this._proxyPool[proxyId1];
              --bound3.StabbingCount;
              if (bound3.IsUpper)
              {
                if (this.TestOverlap(b2, p))
                  this._pairManager.RemoveBufferedPair(proxyId, proxyId1);
                --p.UpperBounds[index1];
                --bound2.StabbingCount;
              }
              else
              {
                --p.LowerBounds[index1];
                ++bound2.StabbingCount;
              }
              ++proxy.LowerBounds[index1];
              Math.Swap<Bound>(ref bound1[index2], ref bound1[index2 + 1]);
            }
          }
          if (num3 < 0)
          {
            for (int index2 = upperBound; index2 > 0 && (int) upperValue < (int) bound1[index2 - 1].Value; --index2)
            {
              Bound bound2 = bound1[index2];
              Bound bound3 = bound1[index2 - 1];
              int proxyId1 = (int) bound3.ProxyId;
              Proxy p = this._proxyPool[proxyId1];
              --bound3.StabbingCount;
              if (bound3.IsLower)
              {
                if (this.TestOverlap(b2, p))
                  this._pairManager.RemoveBufferedPair(proxyId, proxyId1);
                ++p.LowerBounds[index1];
                --bound2.StabbingCount;
              }
              else
              {
                ++p.UpperBounds[index1];
                ++bound2.StabbingCount;
              }
              --proxy.UpperBounds[index1];
              Math.Swap<Bound>(ref bound1[index2], ref bound1[index2 - 1]);
            }
          }
        }
        if (!BroadPhase.IsValidate)
          return;
        this.Validate();
      }
    }

    public void Commit()
    {
      this._pairManager.Commit();
    }

    public Proxy GetProxy(int proxyId)
    {
      if (proxyId == (int) PairManager.NullProxy || !this._proxyPool[proxyId].IsValid)
        return (Proxy) null;
      return this._proxyPool[proxyId];
    }

    public int Query(AABB aabb, object[] userData, int maxCount)
    {
      ushort[] lowerValues;
      ushort[] upperValues;
      this.ComputeBounds(out lowerValues, out upperValues, aabb);
      int lowerQueryOut;
      int upperQueryOut;
      this.Query(out lowerQueryOut, out upperQueryOut, lowerValues[0], upperValues[0], this._bounds[0], 2 * this._proxyCount, 0);
      this.Query(out lowerQueryOut, out upperQueryOut, lowerValues[1], upperValues[1], this._bounds[1], 2 * this._proxyCount, 1);
      Box2DXDebug.Assert(this._queryResultCount < Settings.MaxProxies);
      int num = 0;
      for (int index = 0; index < this._queryResultCount && num < maxCount; ++num)
      {
        Box2DXDebug.Assert((int) this._queryResults[index] < Settings.MaxProxies);
        Proxy proxy = this._proxyPool[(int) this._queryResults[index]];
        Box2DXDebug.Assert(proxy.IsValid);
        userData[index] = proxy.UserData;
        ++index;
      }
      this._queryResultCount = 0;
      this.IncrementTimeStamp();
      return num;
    }

    public unsafe int QuerySegment(Segment segment, object[] userData, int maxCount, SortKeyFunc sortKey)
    {
      float num1 = 1f;
      float num2 = (segment.P2.X - segment.P1.X) * this._quantizationFactor.X;
      float num3 = (segment.P2.Y - segment.P1.Y) * this._quantizationFactor.Y;
      int num4 = (double) num2 < -(double) Settings.FLT_EPSILON ? -1 : ((double) num2 > (double) Settings.FLT_EPSILON ? 1 : 0);
      int num5 = (double) num3 < -(double) Settings.FLT_EPSILON ? -1 : ((double) num3 > (double) Settings.FLT_EPSILON ? 1 : 0);
      Box2DXDebug.Assert(num4 != 0 || num5 != 0);
      float num6 = (segment.P1.X - this._worldAABB.LowerBound.X) * this._quantizationFactor.X;
      float num7 = (segment.P1.Y - this._worldAABB.LowerBound.Y) * this._quantizationFactor.Y;
      ushort* numPtr1 = stackalloc ushort[2];
      ushort* numPtr2 = stackalloc ushort[2];
      numPtr1[0] = (ushort) ((uint) (ushort) num6 & (uint) BroadPhase.BROADPHASE_MAX - 1U);
      numPtr2[0] = (ushort) ((uint) (ushort) num6 | 1U);
      numPtr1[1] = (ushort) ((uint) (ushort) num7 & (uint) BroadPhase.BROADPHASE_MAX - 1U);
      numPtr2[1] = (ushort) ((uint) (ushort) num7 | 1U);
      int lowerQueryOut;
      int upperQueryOut;
      this.Query(out lowerQueryOut, out upperQueryOut, numPtr1[0], numPtr2[0], this._bounds[0], 2 * this._proxyCount, 0);
      int index1 = num4 < 0 ? lowerQueryOut : upperQueryOut - 1;
      this.Query(out lowerQueryOut, out upperQueryOut, numPtr1[1], numPtr2[1], this._bounds[1], 2 * this._proxyCount, 1);
      int index2 = num5 < 0 ? lowerQueryOut : upperQueryOut - 1;
      if (sortKey != null)
      {
        for (int index3 = 0; index3 < this._queryResultCount; ++index3)
          this._querySortKeys[index3] = sortKey(this._proxyPool[(int) this._queryResults[index3]].UserData);
        int index4 = 0;
        while (index4 < this._queryResultCount - 1)
        {
          float querySortKey1 = this._querySortKeys[index4];
          float querySortKey2 = this._querySortKeys[index4 + 1];
          if (((double) querySortKey1 < 0.0 ? ((double) querySortKey2 >= 0.0 ? 1 : 0) : ((double) querySortKey1 <= (double) querySortKey2 ? 0 : ((double) querySortKey2 >= 0.0 ? 1 : 0))) != 0)
          {
            this._querySortKeys[index4 + 1] = querySortKey1;
            this._querySortKeys[index4] = querySortKey2;
            ushort queryResult = this._queryResults[index4 + 1];
            this._queryResults[index4 + 1] = this._queryResults[index4];
            this._queryResults[index4] = queryResult;
            --index4;
            if (index4 == -1)
              index4 = 1;
          }
          else
            ++index4;
        }
        while (this._queryResultCount > 0 && (double) this._querySortKeys[this._queryResultCount - 1] < 0.0)
          --this._queryResultCount;
      }
      bool flag = true;
      float num8 = 0.0f;
      float num9 = 0.0f;
      if (index1 >= 0 && index1 < this._proxyCount * 2 && (index2 >= 0 && index2 < this._proxyCount * 2))
      {
        if (num4 != 0)
        {
          if (num4 > 0)
          {
            ++index1;
            if (index1 == this._proxyCount * 2)
              goto label_66;
          }
          else
          {
            --index1;
            if (index1 < 0)
              goto label_66;
          }
          num8 = ((float) this._bounds[0][index1].Value - num6) / num2;
        }
        if (num5 != 0)
        {
          if (num5 > 0)
          {
            ++index2;
            if (index2 == this._proxyCount * 2)
              goto label_66;
          }
          else
          {
            --index2;
            if (index2 < 0)
              goto label_66;
          }
          num9 = ((float) this._bounds[1][index2].Value - num7) / num3;
        }
        while (true)
        {
          flag = true;
          if (num5 == 0 || num4 != 0 && (double) num8 < (double) num9)
          {
            if ((double) num8 <= (double) num1)
            {
              if ((num4 > 0 ? (this._bounds[0][index1].IsLower ? 1 : 0) : (this._bounds[0][index1].IsUpper ? 1 : 0)) != 0)
              {
                ushort proxyId = this._bounds[0][index1].ProxyId;
                Proxy proxy = this._proxyPool[(int) proxyId];
                if (num5 >= 0)
                {
                  if ((int) proxy.LowerBounds[1] <= index2 - 1 && (int) proxy.UpperBounds[1] >= index2)
                  {
                    if (sortKey != null)
                    {
                      this.AddProxyResult(proxyId, proxy, maxCount, sortKey);
                    }
                    else
                    {
                      this._queryResults[this._queryResultCount] = proxyId;
                      ++this._queryResultCount;
                    }
                  }
                }
                else if ((int) proxy.LowerBounds[1] <= index2 && (int) proxy.UpperBounds[1] >= index2 + 1)
                {
                  if (sortKey != null)
                  {
                    this.AddProxyResult(proxyId, proxy, maxCount, sortKey);
                  }
                  else
                  {
                    this._queryResults[this._queryResultCount] = proxyId;
                    ++this._queryResultCount;
                  }
                }
              }
              if (sortKey == null || this._queryResultCount != maxCount || this._queryResultCount <= 0 || (double) num8 <= (double) this._querySortKeys[this._queryResultCount - 1])
              {
                if (num4 > 0)
                {
                  ++index1;
                  if (index1 == this._proxyCount * 2)
                    break;
                }
                else
                {
                  --index1;
                  if (index1 < 0)
                    break;
                }
                num8 = ((float) this._bounds[0][index1].Value - num6) / num2;
              }
              else
                break;
            }
            else
              break;
          }
          else if ((double) num9 <= (double) num1)
          {
            if ((num5 > 0 ? (this._bounds[1][index2].IsLower ? 1 : 0) : (this._bounds[1][index2].IsUpper ? 1 : 0)) != 0)
            {
              ushort proxyId = this._bounds[1][index2].ProxyId;
              Proxy proxy = this._proxyPool[(int) proxyId];
              if (num4 >= 0)
              {
                if ((int) proxy.LowerBounds[0] <= index1 - 1 && (int) proxy.UpperBounds[0] >= index1)
                {
                  if (sortKey != null)
                  {
                    this.AddProxyResult(proxyId, proxy, maxCount, sortKey);
                  }
                  else
                  {
                    this._queryResults[this._queryResultCount] = proxyId;
                    ++this._queryResultCount;
                  }
                }
              }
              else if ((int) proxy.LowerBounds[0] <= index1 && (int) proxy.UpperBounds[0] >= index1 + 1)
              {
                if (sortKey != null)
                {
                  this.AddProxyResult(proxyId, proxy, maxCount, sortKey);
                }
                else
                {
                  this._queryResults[this._queryResultCount] = proxyId;
                  ++this._queryResultCount;
                }
              }
            }
            if (sortKey == null || this._queryResultCount != maxCount || this._queryResultCount <= 0 || (double) num9 <= (double) this._querySortKeys[this._queryResultCount - 1])
            {
              if (num5 > 0)
              {
                ++index2;
                if (index2 == this._proxyCount * 2)
                  break;
              }
              else
              {
                --index2;
                if (index2 < 0)
                  break;
              }
              num9 = ((float) this._bounds[1][index2].Value - num7) / num3;
            }
            else
              break;
          }
          else
            break;
        }
      }
label_66:
      int num10 = 0;
      for (int index3 = 0; index3 < this._queryResultCount && num10 < maxCount; ++num10)
      {
        Box2DXDebug.Assert((int) this._queryResults[index3] < Settings.MaxProxies);
        Proxy proxy = this._proxyPool[(int) this._queryResults[index3]];
        Box2DXDebug.Assert(proxy.IsValid);
        userData[index3] = proxy.UserData;
        ++index3;
      }
      this._queryResultCount = 0;
      this.IncrementTimeStamp();
      return num10;
    }

    public void Validate()
    {
      for (int index1 = 0; index1 < 2; ++index1)
      {
        Bound[] bound1 = this._bounds[index1];
        int num1 = 2 * this._proxyCount;
        ushort num2 = 0;
        for (int index2 = 0; index2 < num1; ++index2)
        {
          Bound bound2 = bound1[index2];
          Box2DXDebug.Assert(index2 == 0 || (int) bound1[index2 - 1].Value <= (int) bound2.Value);
          Box2DXDebug.Assert((int) bound2.ProxyId != (int) PairManager.NullProxy);
          Box2DXDebug.Assert(this._proxyPool[(int) bound2.ProxyId].IsValid);
          if (bound2.IsLower)
          {
            Box2DXDebug.Assert((int) this._proxyPool[(int) bound2.ProxyId].LowerBounds[index1] == index2);
            ++num2;
          }
          else
          {
            Box2DXDebug.Assert((int) this._proxyPool[(int) bound2.ProxyId].UpperBounds[index1] == index2);
            --num2;
          }
          Box2DXDebug.Assert((int) bound2.StabbingCount == (int) num2);
        }
      }
    }

    private void ComputeBounds(out ushort[] lowerValues, out ushort[] upperValues, AABB aabb)
    {
      lowerValues = new ushort[2];
      upperValues = new ushort[2];
      Box2DXDebug.Assert((double) aabb.UpperBound.X >= (double) aabb.LowerBound.X);
      Box2DXDebug.Assert((double) aabb.UpperBound.Y >= (double) aabb.LowerBound.Y);
      Vec2 vec2_1 = Math.Clamp(aabb.LowerBound, this._worldAABB.LowerBound, this._worldAABB.UpperBound);
      Vec2 vec2_2 = Math.Clamp(aabb.UpperBound, this._worldAABB.LowerBound, this._worldAABB.UpperBound);
      lowerValues[0] = (ushort) ((uint) (ushort) ((double) this._quantizationFactor.X * ((double) vec2_1.X - (double) this._worldAABB.LowerBound.X)) & (uint) BroadPhase.BROADPHASE_MAX - 1U);
      upperValues[0] = (ushort) ((uint) (ushort) ((double) this._quantizationFactor.X * ((double) vec2_2.X - (double) this._worldAABB.LowerBound.X)) | 1U);
      lowerValues[1] = (ushort) ((uint) (ushort) ((double) this._quantizationFactor.Y * ((double) vec2_1.Y - (double) this._worldAABB.LowerBound.Y)) & (uint) BroadPhase.BROADPHASE_MAX - 1U);
      upperValues[1] = (ushort) ((uint) (ushort) ((double) this._quantizationFactor.Y * ((double) vec2_2.Y - (double) this._worldAABB.LowerBound.Y)) | 1U);
    }

    internal bool TestOverlap(Proxy p1, Proxy p2)
    {
      for (int index = 0; index < 2; ++index)
      {
        Bound[] bound = this._bounds[index];
        Box2DXDebug.Assert((int) p1.LowerBounds[index] < 2 * this._proxyCount);
        Box2DXDebug.Assert((int) p1.UpperBounds[index] < 2 * this._proxyCount);
        Box2DXDebug.Assert((int) p2.LowerBounds[index] < 2 * this._proxyCount);
        Box2DXDebug.Assert((int) p2.UpperBounds[index] < 2 * this._proxyCount);
        if ((int) bound[(int) p1.LowerBounds[index]].Value > (int) bound[(int) p2.UpperBounds[index]].Value || (int) bound[(int) p1.UpperBounds[index]].Value < (int) bound[(int) p2.LowerBounds[index]].Value)
          return false;
      }
      return true;
    }

    internal bool TestOverlap(BoundValues b, Proxy p)
    {
      for (int index = 0; index < 2; ++index)
      {
        Bound[] bound = this._bounds[index];
        Box2DXDebug.Assert((int) p.LowerBounds[index] < 2 * this._proxyCount);
        Box2DXDebug.Assert((int) p.UpperBounds[index] < 2 * this._proxyCount);
        if ((int) b.LowerValues[index] > (int) bound[(int) p.UpperBounds[index]].Value || (int) b.UpperValues[index] < (int) bound[(int) p.LowerBounds[index]].Value)
          return false;
      }
      return true;
    }

    private void Query(out int lowerQueryOut, out int upperQueryOut, ushort lowerValue, ushort upperValue, Bound[] bounds, int boundCount, int axis)
    {
      int num1 = BroadPhase.BinarySearch(bounds, boundCount, lowerValue);
      int num2 = BroadPhase.BinarySearch(bounds, boundCount, upperValue);
      for (int index = num1; index < num2; ++index)
      {
        if (bounds[index].IsLower)
          this.IncrementOverlapCount((int) bounds[index].ProxyId);
      }
      if (num1 > 0)
      {
        int index = num1 - 1;
        int stabbingCount = (int) bounds[index].StabbingCount;
        while (stabbingCount != 0)
        {
          Box2DXDebug.Assert(index >= 0);
          if (bounds[index].IsLower)
          {
            Proxy proxy = this._proxyPool[(int) bounds[index].ProxyId];
            if (num1 <= (int) proxy.UpperBounds[axis])
            {
              this.IncrementOverlapCount((int) bounds[index].ProxyId);
              --stabbingCount;
            }
          }
          --index;
        }
      }
      lowerQueryOut = num1;
      upperQueryOut = num2;
    }

    private void IncrementOverlapCount(int proxyId)
    {
      Proxy proxy = this._proxyPool[proxyId];
      if ((int) proxy.TimeStamp < (int) this._timeStamp)
      {
        proxy.TimeStamp = this._timeStamp;
        proxy.OverlapCount = (ushort) 1;
        ++this.qi1;
      }
      else
      {
        proxy.OverlapCount = (ushort) 2;
        Box2DXDebug.Assert(this._queryResultCount < Settings.MaxProxies);
        this._queryResults[this._queryResultCount] = (ushort) proxyId;
        ++this._queryResultCount;
        ++this.qi2;
      }
    }

    private void IncrementTimeStamp()
    {
      if ((int) this._timeStamp == (int) BroadPhase.BROADPHASE_MAX)
      {
        for (ushort index = 0; (int) index < Settings.MaxProxies; ++index)
          this._proxyPool[(int) index].TimeStamp = (ushort) 0;
        this._timeStamp = (ushort) 1;
      }
      else
        ++this._timeStamp;
    }

    public unsafe void AddProxyResult(ushort proxyId, Proxy proxy, int maxCount, SortKeyFunc sortKey)
    {
      float num = sortKey(proxy.UserData);
      if ((double) num < 0.0)
        return;
      fixed (float* numPtr1 = this._querySortKeys)
      {
        float* numPtr2 = numPtr1;
        while ((double) *numPtr2 < (double) num && numPtr2 < numPtr1 + this._queryResultCount)
          ++numPtr2;
        int index1 = (int) (numPtr2 - numPtr1);
        if (maxCount == this._queryResultCount && index1 == this._queryResultCount)
          return;
        if (maxCount == this._queryResultCount)
          --this._queryResultCount;
        for (int index2 = this._queryResultCount + 1; index2 > index1; --index2)
        {
          this._querySortKeys[index2] = this._querySortKeys[index2 - 1];
          this._queryResults[index2] = this._queryResults[index2 - 1];
        }
        this._querySortKeys[index1] = num;
        this._queryResults[index1] = proxyId;
        ++this._queryResultCount;
      }
    }

    private static int BinarySearch(Bound[] bounds, int count, ushort value)
    {
      int num1 = 0;
      int num2 = count - 1;
      while (num1 <= num2)
      {
        int index = num1 + num2 >> 1;
        if ((int) bounds[index].Value > (int) value)
        {
          num2 = index - 1;
        }
        else
        {
          if ((int) bounds[index].Value >= (int) value)
            return (int) (ushort) index;
          num1 = index + 1;
        }
      }
      return num1;
    }
  }
}