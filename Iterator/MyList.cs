using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iterator
{
    internal class MyList<T> : IEnumerable<T>
    {
        private const int DCapacity = 8;           //기본 list의 최대크기
        private T[] items;                               //리스트
        private int size;                                  //원소가 들어가 있는 장소갯수 (Index)

        public MyList()
        {
            this.items = new T[DCapacity];
            this.size = 0;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= size)     //예외처리
                    throw new ArgumentOutOfRangeException();
                return items[index];                 //리스트에서 불러올 값
            }
            set
            {
                if (index < 0 || index >= size)    //예외처리
                    throw new ArgumentOutOfRangeException();
                items[index] = value;                //리스트에 저장할 값
            }
        }
        public int Count                           //리스트에 원소가 들어가 있는 갯수를 반환하는 함수
        {
            get
            {
                return size;
            }
        }
        public int Capacity()                      //리스트의 실질적 값을 반환하는 함수
        {
            return items.Length;
        }
        public void Clear()                       //리스트 초기화하는 함수
        {
            T[] newItems = new T[DCapacity];
            items = newItems;
            size = 0;
        }

        public void Add(T item)               //리스트에 값넣는 함수
        {
            if (size < items.Length)           //값을 넣을 Index가 배열의 크기보다 큰지 계산
            {
                items[size++] = item;         //Index에 값넣고 덮어쓰지 않기위해 size값 키움
            }
            else                                      //배열의 크기보다 size가 큰 경우 오류가 나기때문에 grow함수 적용
            {
                Grow();                             //배열을 늘리는 함수
                items[size++] = item;
            }
        }
        public bool Remove(T item)          //리스트에서 값빼고 뺏으면 true 값이 없어서 못빼면 false를 내는 bool함수
        {
            int index = IndexOf(item);      //item값이 리스트에 있는지 확인
            if (index >= 0)                         //있는 경우
            {
                RemoveAt(index);               //리스트에서 item제거
                return true;                        //true값 반환
            }
            else
            {
                //못 찾은 경우
                return false;
            }
        }
        public void RemoveAt(int index)                                      //위치에 있는 값 제거하는 함수
        {
            if (index < 0 || index >= size)                                       //예외처리
                throw new IndexOutOfRangeException();
            size--;
            Array.Copy(items, index + 1, items, index, size - index);       //리스트에서 값 제거하고 새로 만드는 함수
        }

        public int IndexOf(T item)                                     //리스트에 item값이 있는지 확인하고 그 Index값을 반환하는 함수
        {
            return Array.IndexOf(items, item, 0, size);
        }

        public T? Find(Predicate<T> match)                          //리스트에 조건에 맞는 값 반환하는 함수
        {
            if (match == null)                                                  //예외처리
                throw new ArgumentNullException("match");

            for (int i = 0; i < size; i++)                                    //맨 앞부터 찾음
            {
                if (match(items[i]))
                    return items[i];
            }
            return default(T);                                                //없으면 기본값 반환
        }
        public T? FindLast(Predicate<T> match)                       //리스트에 조건에 맞는 값 뒤에서부터 찾는 함수
        {
            if (match == null)                                                      //예외처리
                throw new ArgumentNullException("match");

            for (int i = size; i < 0; i--)                                         //뒤에부터 찾음
            {
                if (match(items[i]))
                    return items[i];
            }
            return default(T);                                                    //없으면 기본값 반환
        }

        public int FindIndex(Predicate<T> match)                    //조건에 맞는 값의 위치 반환하는 함수
        {
            for (int i = 0; i < size; i++)
            {
                if (match(items[i]))
                    return i;
            }
            return -1;                                                                 //없으면 -1반환
        }
        public int FindLastIndex(Predicate<T> match)             //조건에 맞는 값중 맨 뒤에있는 값의 위치 반환하는 함수
        {
            for (int i = size; i < 0; i--)
            {
                if (match(items[i]))
                    return i;
            }
            return -1;                                                                //맞는 조건이 없으면 -1반환
        }
        private void Grow()                                                    // 더 큰 배열을 만든 후 그 전에 있던 값을 복사하는 함수
        {
            int newCapacity = items.Length * 2;
            T[] newItems = new T[newCapacity];
            Array.Copy(items, 0, newItems, 0, size);
            items = newItems;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MyEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MyEnumerator(this);
        }
        public struct MyEnumerator : IEnumerator<T>
        {
            private MyList<T> list;
            private int index;
            private T current;

            internal MyEnumerator(MyList<T> list)
            {
                this.list = list;
                this.index = 0;
                this.current = default(T);
            }

            public T Current { get { return current; } }

            object IEnumerator.Current
            {
                get
                {
                    if (index < 0 || index >= list.Count)
                        throw new InvalidOperationException();
                    return Current;
                }
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                if (index < list.Count)
                {
                    current = list[index++];
                    return true;
                }
                else
                {
                    current = default(T);
                    index = list.Count;
                    return false;
                }
            }

            public void Reset()
            {
                index = 0;
                current = default(T);
            }
        }
    }
}
/* 반복자는 복잡한 데이터 구조의 내부 세부 정보를 노출하지 않고 해당 구조를 차례대로 순회할 수 있도록 하는 행동 디자인 패턴입니다.

반복자 덕분에 클라이언트들은 단일 반복기 인터페이스를 사용하여 유사한 방식으로 다른 컬렉션들의 요소들을 탐색할 수 있습니다.*/