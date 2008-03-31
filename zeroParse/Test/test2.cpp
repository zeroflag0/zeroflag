AnimationState::AnimationState(const String& animName, 
	AnimationStateSet *parent, Real timePos, Real length, Real weight, 
	bool enabled)
    : mAnimationName(animName)
    , mParent(parent)
    , mTimePos(timePos)
    , mLength(length)
    , mWeight(weight)
    , mEnabled(enabled)
    , mLoop(true)
{
	mParent->_notifyDirty();
}
void AnimationState::setTimePosition(Real timePos)
{
	if (timePos != mTimePos)
	{
		mTimePos = timePos;
		if (mLoop)
		{
			// Wrap
			mTimePos = fmod(mTimePos, mLength);
			if(mTimePos < 0)
				mTimePos += mLength;     
		}
		else
		{
			// Clamp
			if(mTimePos < 0)
				mTimePos = 0;
			else if (mTimePos > mLength)
				mTimePos = mLength;
		}

        if (mEnabled)
            mParent->_notifyDirty();
    }

}
