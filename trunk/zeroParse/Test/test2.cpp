    AnimationTrack::AnimationTrack(Animation* parent, unsigned short handle) :
		mParent(parent), mHandle(handle)
    {
    }
    //---------------------------------------------------------------------
    AnimationTrack::~AnimationTrack()
    {
        removeAllKeyFrames();
    }
    //---------------------------------------------------------------------
    unsigned short AnimationTrack::getNumKeyFrames(void) const
    {
        return (unsigned short)mKeyFrames.size();
    }
    //---------------------------------------------------------------------
    KeyFrame* AnimationTrack::getKeyFrame(unsigned short index) const
    {
		// If you hit this assert, then the keyframe index is out of bounds
        assert( index < (ushort)mKeyFrames.size() );

        return mKeyFrames[index];
    }
    //---------------------------------------------------------------------
    Real AnimationTrack::getKeyFramesAtTime(const TimeIndex& timeIndex, KeyFrame** keyFrame1, KeyFrame** keyFrame2,
        unsigned short* firstKeyIndex) const
    {
        // Parametric time
        // t1 = time of previous keyframe
        // t2 = time of next keyframe
        Real t1, t2;

        Real timePos = timeIndex.getTimePos();

        // Find first keyframe after or on current time
        KeyFrameList::const_iterator i;
        if (timeIndex.hasKeyIndex())
        {
            // Global keyframe index available, map to local keyframe index directly.
            assert(timeIndex.getKeyIndex() < mKeyFrameIndexMap.size());
            i = mKeyFrames.begin() + mKeyFrameIndexMap[timeIndex.getKeyIndex()];
#ifdef _DEBUG
            KeyFrame timeKey(0, timePos);
            if (i != std::lower_bound(mKeyFrames.begin(), mKeyFrames.end(), &timeKey, KeyFrameTimeLess()))
            {
                OGRE_EXCEPT(Exception::ERR_INTERNAL_ERROR,
                    "Optimised key frame search failed",
                    "AnimationTrack::getKeyFramesAtTime");
            }
#endif
        }
        else
        {
            // Wrap time
            Real totalAnimationLength = mParent->getLength();
            assert(totalAnimationLength > 0.0f && "Invalid animation length!");

            while (timePos > totalAnimationLength && totalAnimationLength > 0.0f)
            {
                timePos -= totalAnimationLength;
            }

            // No global keyframe index, need to search with local keyframes.
            KeyFrame timeKey(0, timePos);
            i = std::lower_bound(mKeyFrames.begin(), mKeyFrames.end(), &timeKey, KeyFrameTimeLess());
        }

        if (i == mKeyFrames.end())
        {
            // There is no keyframe after this time, wrap back to first
            *keyFrame2 = mKeyFrames.front();
            t2 = mParent->getLength() + (*keyFrame2)->getTime();

            // Use last keyframe as previous keyframe
            --i;
        }
        else
        {
            *keyFrame2 = *i;
            t2 = (*keyFrame2)->getTime();

            // Find last keyframe before or on current time
            if (i != mKeyFrames.begin() && timePos < (*i)->getTime())
            {
                --i;
            }
        }

        // Fill index of the first key
        if (firstKeyIndex)
        {
            *firstKeyIndex = std::distance(mKeyFrames.begin(), i);
        }

        *keyFrame1 = *i;

        t1 = (*keyFrame1)->getTime();

        if (t1 == t2)
        {
            // Same KeyFrame (only one)
            return 0.0;
        }
        else
        {
            return (timePos - t1) / (t2 - t1);
        }
    }