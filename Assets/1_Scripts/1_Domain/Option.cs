using System;

public readonly struct Option<T>
{
    private readonly T value;
    private readonly bool isSome;

    public bool IsSome => isSome;
    public bool IsNone => !isSome;

    public T Value
    {
        get
        {
            if (!isSome)
            {
                throw new InvalidOperationException("Option is None.");
            }
            return value;
        }
    }

    private Option(T initialValue, bool initialIsSome)
    {
        value = initialValue;
        isSome = initialIsSome;
    }

    // 성공 상태를 반환하는 정적 팩토리 메서드
    public static Option<T> Some(T someValue)
    {
        return new Option<T>(someValue, true);
    }

    // 실패 상태를 반환하는 정적 프로퍼티
    public static Option<T> None => new Option<T>(default, false);

    // Map: 성공 상태일 때만 내부 값을 변환하여 새로운 Option 반환
    public Option<U> Map<U>(Func<T, U> mapper)
    {
        if (!isSome)
        {
            return Option<U>.None;
        }
        U _mappedValue = mapper(value);
        return Option<U>.Some(_mappedValue);
    }

    // Bind: 성공 상태일 때만 다음 파이프라인(Option을 반환하는 함수)을 실행
    public Option<U> Bind<U>(Func<T, Option<U>> binder)
    {
        if (!isSome)
        {
            return Option<U>.None;
        }

        // 지역 변수: _camelCase
        Option<U> _boundValue = binder(value);
        return _boundValue;
    }

    // Match: 파이프라인의 최종 단계에서 성공/실패 여부에 따라 최종 결과값을 추출
    public U Match<U>(Func<T, U> onSome, Func<U> onNone)
    {
        if (isSome)
        {
            U _someResult = onSome(value);
            return _someResult;
        }

        U _noneResult = onNone();
        return _noneResult;
    }
}