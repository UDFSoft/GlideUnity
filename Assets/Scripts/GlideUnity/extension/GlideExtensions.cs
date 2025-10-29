using R3;
using UnityEngine;

namespace Assets.Libs.Glide.Scripts.extension
{
    public static class GlideExtensions
    {
        public static Observable<Sprite> ToSpriteObservable(this GlideRequestBuilder request)
        {
            return Observable.Create<Sprite>(observer =>
            {
                request.Into(sprite =>
                {
                    observer.OnNext(sprite);
                    observer.OnCompleted();
                });

                return Disposable.Empty;
            });
        }
    }
}