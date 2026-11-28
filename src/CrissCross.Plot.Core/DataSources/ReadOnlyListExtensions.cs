// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.DataSources;

/// <summary>Provides allocation-conscious views over read-only lists.</summary>
internal static class ReadOnlyListExtensions
{
    /// <summary>Extension members for <c>IReadOnlyList&lt;T&gt;</c>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="inputList">The list receiving the extension members.</param>
    extension<T>(IReadOnlyList<T> inputList)
    {
        /// <summary>Creates a view that maps each element on access.</summary>
        /// <typeparam name="TOutput">The projected element type.</typeparam>
        /// <param name="mappingFunction">The function applied to each element.</param>
        /// <returns>A read-only projection view.</returns>
        internal IReadOnlyList<TOutput> SelectView<TOutput>(Func<T, TOutput> mappingFunction) => new SelectListView<T, TOutput>(inputList, mappingFunction);

        /// <summary>Creates a view that grows this list by applying a mapping function.</summary>
        /// <param name="growthFactor">The number of output elements generated per input element.</param>
        /// <param name="growthFunction">The function that generates each output element.</param>
        /// <returns>A read-only growth view.</returns>
        internal IReadOnlyList<T> GrowView(
            uint growthFactor,
            Func<int, int, IReadOnlyList<T>, T> growthFunction) => new GrowListView<T>(inputList, growthFactor, growthFunction);

        /// <summary>Creates a view that skips the specified number of elements.</summary>
        /// <param name="toSkip">The number of elements to skip.</param>
        /// <returns>A read-only skip view.</returns>
        internal IReadOnlyList<T> SkipView(int toSkip) => new SkipListView<T>(inputList, Math.Max(0, toSkip));

        /// <summary>Creates a view that exposes at most the specified number of elements.</summary>
        /// <param name="toTake">The maximum number of elements to expose.</param>
        /// <returns>A read-only take view.</returns>
        internal IReadOnlyList<T> TakeView(int toTake) => new TakeListView<T>(inputList, Math.Max(0, toTake));
    }

    /// <summary>Extension members for <c>IReadOnlyList&lt;TFirst&gt;</c>.</summary>
    /// <typeparam name="TFirst">The element type of the first list.</typeparam>
    /// <param name="inputList">The first list receiving the extension members.</param>
    extension<TFirst>(IReadOnlyList<TFirst> inputList)
    {
        /// <summary>Creates a view that zips this list with another list.</summary>
        /// <typeparam name="TSecond">The element type of the second list.</typeparam>
        /// <typeparam name="TOutput">The combined element type.</typeparam>
        /// <param name="toZipWith">The second list.</param>
        /// <param name="zippingFunction">The function that combines corresponding elements.</param>
        /// <returns>A read-only zip view.</returns>
        internal IReadOnlyList<TOutput> ZipView<TSecond, TOutput>(
            IReadOnlyList<TSecond> toZipWith,
            Func<TFirst, TSecond, TOutput> zippingFunction) => new ZipListView<TFirst, TSecond, TOutput>(inputList, toZipWith, zippingFunction);
    }

    /// <summary>
    /// List view which projects elements from the original list onto a new form.
    /// A view-only equivalent of Enumerable#Select(...).
    /// </summary>
    /// <typeparam name="TInput">The type of the original list elements.</typeparam>
    /// <typeparam name="TOutput">The type of the output elements.</typeparam>
    /// <param name="inputList">The original list.</param>
    /// <param name="mappingFunction">The function used to map elements.</param>
    private sealed class SelectListView<TInput, TOutput>(IReadOnlyList<TInput> inputList, Func<TInput, TOutput> mappingFunction) : ListViewDecorator<TOutput>
    {
        /// <summary>The original list.</summary>
        private readonly IReadOnlyList<TInput> _inputList = inputList;

        /// <summary>The element mapping function.</summary>
        private readonly Func<TInput, TOutput> _mappingFunction = mappingFunction;

        /// <summary>Gets the number of elements in this view.</summary>
        public override int Count => _inputList.Count;

        /// <summary>Maps and gets the element at the specified index.</summary>
        public override TOutput this[int index] => _mappingFunction.Invoke(_inputList[index]);
    }

    /// <summary>
    /// List view which grows the number of elements by a positive integer factor.
    /// A growth function is applied to each element such that element 0 could expand into
    /// element 0, 1, 2, ..., (_growthFactor - 1).
    /// </summary>
    /// <example>
    /// The below example illustrates using this view to double the size of a list of strings,
    /// adding the number of each duplicate to the end of each string.
    /// <code>
    /// <![CDATA[
    /// IReadOnlyList<string> original = new string[] {"A", "B", "C"};
    /// print(original); //"A", "B", "C"
    /// IReadOnlyList<string> grown = new GrowListView<string>(
    ///     original, 3, (step, originalIndex, originalList) => $"{originalList[originalIndex]}-{step}");
    /// print(grown); //"A-0", "A-1", "A-2", "B-0", "B-1", "B-2", "C-0", "C-1", "C-2"
    /// ]]>
    /// </code>
    /// </example>
    /// <typeparam name="T">The type of the input elements.</typeparam>
    /// <param name="inputList">The original list.</param>
    /// <param name="growthFactor">The number of output elements generated per input element.</param>
    /// <param name="growthFunction">The function that generates each output element.</param>
    private sealed class GrowListView<T>(
            IReadOnlyList<T> inputList,
            uint growthFactor,
            Func<int, int, IReadOnlyList<T>, T> growthFunction) : ListViewDecorator<T>
    {
        /// <summary>The original list.</summary>
        private readonly IReadOnlyList<T> _inputList = inputList;

        /// <summary>The number of output elements generated per input element.</summary>
        private readonly uint _growthFactor = growthFactor;

        /// <summary>The function that generates output elements.</summary>
        private readonly Func<int, int, IReadOnlyList<T>, T> _growthFunction = growthFunction;

        /// <summary>Gets the number of elements in this view.</summary>
        public override int Count => (int)(_inputList.Count * _growthFactor);

        /// <summary>Gets the generated element at the specified index.</summary>
        public override T this[int index] => _growthFunction.Invoke(
            (int)(index % _growthFactor), // current step (starts at zero)
            (int)Math.Floor((double)index / _growthFactor), // original index
            _inputList);
    }

    /// <summary>List view which zips two lists together by applying a mapping function. A view-only equivalent of Enumerable#Zip(...).</summary>
    /// <typeparam name="TFirst">The element type of the first input list.</typeparam>
    /// <typeparam name="TSecond">The element type of the second input list.</typeparam>
    /// <typeparam name="TOutput">The type of the combined output elements.</typeparam>
    /// <param name="firstInput">The first input list.</param>
    /// <param name="secondInput">The second input list.</param>
    /// <param name="zippingFunction">The function that combines corresponding elements.</param>
    private sealed class ZipListView<TFirst, TSecond, TOutput>(
            IReadOnlyList<TFirst> firstInput,
            IReadOnlyList<TSecond> secondInput,
            Func<TFirst, TSecond, TOutput> zippingFunction) : ListViewDecorator<TOutput>
    {
        /// <summary>The first input list.</summary>
        private readonly IReadOnlyList<TFirst> _firstInput = firstInput;

        /// <summary>The second input list.</summary>
        private readonly IReadOnlyList<TSecond> _secondInput = secondInput;

        /// <summary>The function that combines elements.</summary>
        private readonly Func<TFirst, TSecond, TOutput> _zippingFunction = zippingFunction;

        /// <summary>Gets the number of elements in this view.</summary>
        public override int Count => Math.Min(_firstInput.Count, _secondInput.Count);

        /// <summary>Gets the combined element at the specified index.</summary>
        public override TOutput this[int index] =>
            _zippingFunction.Invoke(_firstInput[index], _secondInput[index]);
    }

    /// <summary>List view which skips N elements from the start of the list. A view-only equivalent of Enumerable#Skip(N).</summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <param name="originalList">The original list.</param>
    /// <param name="toSkip">The number of elements to skip.</param>
    private sealed class SkipListView<T>(IReadOnlyList<T> originalList, int toSkip) : ListViewDecorator<T>
    {
        /// <summary>The original list.</summary>
        private readonly IReadOnlyList<T> _originalList = originalList;

        /// <summary>The number of elements skipped.</summary>
        private readonly int _toSkip = toSkip;

        /// <summary>Gets the number of elements in this view.</summary>
        public override int Count => Math.Max(_originalList.Count - _toSkip, 0);

        /// <summary>Gets the source element adjusted for the skipped prefix.</summary>
        public override T this[int index]
        {
            get
            {
                int mappedIndex = index + _toSkip;
                return _originalList[mappedIndex];
            }
        }
    }

    /// <summary>List view which takes N elements from the start of the list. A view-only equivalent of Enumerable#Take(N).</summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <param name="originalList">The original list.</param>
    /// <param name="toTake">The maximum number of elements exposed.</param>
    private sealed class TakeListView<T>(IReadOnlyList<T> originalList, int toTake) : ListViewDecorator<T>
    {
        /// <summary>The original list.</summary>
        private readonly IReadOnlyList<T> _originalList = originalList;

        /// <summary>The maximum number of elements exposed.</summary>
        private readonly int _toTake = toTake;

        /// <summary>Gets the number of elements in this view.</summary>
        public override int Count => Math.Min(_originalList.Count, _toTake);

        /// <summary>Gets the source element at the specified index.</summary>
        public override T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _originalList[index];
            }
        }
    }

    /// <summary>Provides enumeration behavior for read-only list views.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    private abstract class ListViewDecorator<T> : IReadOnlyList<T>
    {
        /// <summary>Gets the number of elements in the view.</summary>
        public abstract int Count { get; }

        /// <summary>Gets the element at the specified index.</summary>
        public abstract T this[int index] { get; }

        /// <summary>Gets an enumerator for the elements in the view.</summary>
        /// <returns>An enumerator over the view.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
